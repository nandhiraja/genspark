using System;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(Guid bookingId, decimal amount);
    }
}
