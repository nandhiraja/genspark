using BusBooking.Backend.DTOs;
using System;
using System.Threading.Tasks;

namespace BusBooking.Backend.Services
{
    public interface IBookingService
    {
        Task<bool> LockSeatsAsync(Guid userId, LockSeatsRequestDto request);
        Task<Guid?> ConfirmBookingAsync(Guid userId, ConfirmBookingRequestDto request);
        Task<bool> CancelBookingAsync(Guid userId, Guid bookingId);
        Task<bool> UnlockSeatsAsync(Guid userId, Guid busId, List<Guid> seatIds);
    }
}
