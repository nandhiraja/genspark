using BusBooking.Backend.DTOs;
using BusBooking.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BusBooking.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("lock-seats")]
        public async Task<IActionResult> LockSeats([FromBody] LockSeatsRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _bookingService.LockSeatsAsync(userId, request);
            
            if (!result) return BadRequest(new { message = "Failed to lock seats. Some seats might be already booked or locked." });
            return Ok(new { message = "Seats locked successfully for 5 minutes." });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _bookingService.ConfirmBookingAsync(userId, request);

            if (!result) return BadRequest(new { message = "Booking failed. Ensure seats are locked and payment is valid." });
            return Ok(new { message = "Booking confirmed." });
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _bookingService.CancelBookingAsync(userId, id);

            if (!result) return BadRequest(new { message = "Cannot cancel this booking." });
            return Ok(new { message = "Booking cancelled." });
        }
    }
}
