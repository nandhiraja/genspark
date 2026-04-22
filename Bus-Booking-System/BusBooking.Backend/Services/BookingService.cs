using BusBooking.Backend.DTOs;
using BusBooking.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;

        public BookingService(ApplicationDbContext context, IPaymentService paymentService, IEmailService emailService)
        {
            _context = context;
            _paymentService = paymentService;
            _emailService = emailService;
        }

        public async Task<bool> LockSeatsAsync(Guid userId, LockSeatsRequestDto request)
        {
            foreach (var seatId in request.SeatIds)
            {
                var isBooked = await _context.Tickets.AnyAsync(t => t.SeatId == seatId && t.BookingId != Guid.Empty);
                if (isBooked) return false;

                var activeLock = await _context.SeatLocks
                    .Where(sl => sl.SeatId == seatId && sl.ExpiryTime > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (activeLock != null && activeLock.UserId != userId) return false;
            }

            foreach (var seatId in request.SeatIds)
            {
                var seatLock = new SeatLock
                {
                    SeatId = seatId,
                    BusId = request.BusId,
                    UserId = userId,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5)
                };
                await _context.SeatLocks.AddAsync(seatLock);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmBookingAsync(Guid userId, ConfirmBookingRequestDto request)
        {
            if (request.SeatIds.Count != request.Passengers.Count) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var locks = await _context.SeatLocks
                    .Where(sl => request.SeatIds.Contains(sl.SeatId) && sl.UserId == userId && sl.ExpiryTime > DateTime.UtcNow)
                    .ToListAsync();

                if (locks.Count != request.SeatIds.Count) return false;

                var bus = await _context.Buses.FindAsync(request.BusId);
                if (bus == null) return false;

                var totalAmount = bus.Price * request.SeatIds.Count;

                var booking = new Booking
                {
                    UserId = userId,
                    BusId = request.BusId,
                    TotalAmount = totalAmount,
                    Status = BookingStatus.PENDING
                };

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                var paymentSuccess = await _paymentService.ProcessPaymentAsync(booking.Id, totalAmount);
                if (!paymentSuccess)
                {
                    booking.Status = BookingStatus.CANCELLED;
                    _context.SeatLocks.RemoveRange(locks);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return false;
                }

                booking.Status = BookingStatus.CONFIRMED;

                for (int i = 0; i < request.SeatIds.Count; i++)
                {
                    var ticket = new Ticket
                    {
                        BookingId = booking.Id,
                        SeatId = request.SeatIds[i],
                        Name = request.Passengers[i].Name,
                        Age = request.Passengers[i].Age,
                        Gender = request.Passengers[i].Gender
                    };
                    await _context.Tickets.AddAsync(ticket);
                }

                _context.SeatLocks.RemoveRange(locks);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(user.Email, "Booking Confirmed", $"Your booking for bus {bus.BusNumber} is confirmed!");
                    }
                    catch { /* Ignore email failure to not break booking flow */ }
                }

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> CancelBookingAsync(Guid userId, Guid bookingId)
        {
            var booking = await _context.Bookings.Include(b => b.Bus).FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
            if (booking == null || booking.Status != BookingStatus.CONFIRMED) return false;

            if (booking.Bus != null && booking.Bus.StartTime <= DateTime.UtcNow) return false;

            booking.Status = BookingStatus.CANCELLED;
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Booking Cancelled", "Your booking has been cancelled.");
                }
                catch { /* Ignore email failure */ }
            }

            return true;
        }
    }
}
