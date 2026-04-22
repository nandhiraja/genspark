using System;
using System.Threading.Tasks;
using BusBooking.Backend.Models;

namespace BusBooking.Backend.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessPaymentAsync(Guid bookingId, decimal amount)
        {
            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = amount,
                Status = PaymentStatus.INITIATED
            };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();

            await Task.Delay(2000); // simulate delay

            var random = new Random();
            var isSuccess = random.Next(0, 100) > 20; // 80% success

            payment.Status = isSuccess ? PaymentStatus.SUCCESS : PaymentStatus.FAILED;
            await _context.SaveChangesAsync();

            return isSuccess;
        }
    }
}
