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
                var isBooked = await _context.Tickets
                    .Include(t => t.Booking)
                    .AnyAsync(t => t.SeatId == seatId && t.Booking!.Status == BookingStatus.CONFIRMED);

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

        public async Task<Guid?> ConfirmBookingAsync(Guid userId, ConfirmBookingRequestDto request)
        {
            if (request.SeatIds.Count != request.Passengers.Count) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bus = await _context.Buses.FindAsync(request.BusId);
                if (bus == null) return null;

                var locks = await _context.SeatLocks
                    .Where(sl => request.SeatIds.Contains(sl.SeatId) && sl.UserId == userId && sl.ExpiryTime > DateTime.UtcNow)
                    .ToListAsync();

                if (locks.Count != request.SeatIds.Count) return null;

                const decimal PlatformFeePercent = 5m;
                var totalAmount = Math.Round(bus.Price * request.SeatIds.Count * (1 + PlatformFeePercent / 100), 2);

                var booking = new Booking
                {
                    UserId = userId,
                    BusId = request.BusId,
                    TotalAmount = totalAmount,
                    Status = BookingStatus.CONFIRMED
                };

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                for (int i = 0; i < request.SeatIds.Count; i++)
                {
                    var ticket = new Ticket
                    {
                        BookingId = booking.Id,
                        SeatId = request.SeatIds[i],
                        Name = request.Passengers[i].Name,
                        Age = request.Passengers[i].Age,
                        Gender = request.Passengers[i].Gender,
                        Phone = request.Passengers[i].Phone
                    };
                    await _context.Tickets.AddAsync(ticket);
                }

                _context.SeatLocks.RemoveRange(locks);
                await _context.SaveChangesAsync();

                var confirmedOnBus = await _context.Tickets.CountAsync(t =>
                    t.Seat!.BusId == bus.Id && t.Booking!.Status == BookingStatus.CONFIRMED);
                bus.AvailableSeats = Math.Max(0, bus.TotalSeats - confirmedOnBus);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    try
                    {
                        var paxSummary = string.Join(", ", request.Passengers.Select(p => p.Name));
                        await _emailService.SendEmailAsync(user.Email, "GO-BUS — Booking confirmed",
                            $"Hello,\r\n\r\nYour payment was successful.\r\n\r\nBooking ID: {booking.Id}\r\nBus: {bus.BusNumber}\r\nAmount paid: {totalAmount}\r\n\r\nPassengers: {paxSummary}\r\n\r\nThank you for choosing GO-BUS.");
                    }
                    catch { /* Ignore email failure */ }
                }

                return booking.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> CancelBookingAsync(Guid userId, Guid bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Bus)
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

            if (booking == null || booking.Status != BookingStatus.CONFIRMED) return false;
            if (booking.Bus == null) return false;

            var timeToStart = booking.Bus.StartTime - DateTime.UtcNow;

            if (timeToStart.TotalHours <= 0) return false;

            decimal refundAmount;
            string refundNote;
            if (timeToStart.TotalHours >= 24)
            {
                refundAmount = booking.TotalAmount;
                refundNote = "100% refund (cancelled 24+ hours before departure).";
            }
            else if (timeToStart.TotalHours >= 12)
            {
                refundAmount = Math.Round(booking.TotalAmount * 0.5m, 2);
                refundNote = "50% refund (cancelled between 12 and 24 hours before departure).";
            }
            else
            {
                refundAmount = 0;
                refundNote = "No refund (cancelled less than 12 hours before departure). Your seats are released for other travellers.";
            }

            var tickets = booking.Tickets?.ToList() ?? await _context.Tickets.Where(t => t.BookingId == bookingId).ToListAsync();
            if (tickets.Count > 0)
                _context.Tickets.RemoveRange(tickets);

            booking.Status = BookingStatus.CANCELLED;
            await _context.SaveChangesAsync();

            var busId = booking.Bus.Id;
            var confirmedOnBus = await _context.Tickets.CountAsync(t =>
                t.Seat!.BusId == busId && t.Booking!.Status == BookingStatus.CONFIRMED);
            booking.Bus.AvailableSeats = Math.Max(0, booking.Bus.TotalSeats - confirmedOnBus);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                try
                {
                    await _emailService.SendEmailAsync(user.Email, "GO-BUS — Booking cancelled",
                        $"Your booking {bookingId} has been cancelled.\r\n\r\n{refundNote}\r\nRefund amount credited (mock): ₹{refundAmount}\r\n\r\nReleased seats are available for booking again.");
                }
                catch { /* Ignore email failure */ }
            }

            return true;
        }

        public async Task<bool> UnlockSeatsAsync(Guid userId, Guid busId, List<Guid> seatIds)
        {
            var locks = await _context.SeatLocks
                .Where(sl => sl.UserId == userId && sl.BusId == busId && seatIds.Contains(sl.SeatId))
                .ToListAsync();

            if (locks.Any())
            {
                _context.SeatLocks.RemoveRange(locks);
                await _context.SaveChangesAsync();
            }
            return true;
        }
    }
}
