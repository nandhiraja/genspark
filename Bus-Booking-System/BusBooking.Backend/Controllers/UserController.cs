using BusBooking.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusBooking.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search-buses")]
        public async Task<IActionResult> SearchBuses([FromQuery] string source, [FromQuery] string destination, [FromQuery] DateTime date)
        {
            var buses = await _context.Buses
                .Include(b => b.Route)
                .Include(b => b.Operator)
                .Where(b => b.Route!.Source == source 
                         && b.Route.Destination == destination 
                         && b.StartTime.Date == date.Date 
                         && b.Status == BusStatus.ACTIVE)
                .Select(b => new
                {
                    b.Id,
                    b.BusNumber,
                    b.StartTime,
                    b.EndTime,
                    b.Price,
                    b.TotalSeats,
                    b.AvailableSeats,
                    OperatorName = b.Operator!.CompanyName
                })
                .ToListAsync();

            return Ok(buses);
        }

        [HttpGet("bus-seats/{busId}")]
        public async Task<IActionResult> BusSeats(Guid busId)
        {
            var seats = await _context.Seats
                .Where(s => s.BusId == busId)
                .OrderBy(s => s.SeatNumber)
                .Select(s => new
                {
                    s.Id,
                    s.SeatNumber,
                    Status = _context.Tickets.Any(t => t.SeatId == s.Id) ? "BOOKED"
                           : _context.SeatLocks.Any(sl => sl.SeatId == s.Id && sl.ExpiryTime > DateTime.UtcNow) ? "LOCKED"
                           : "AVAILABLE"
                })
                .ToListAsync();

            return Ok(seats);
        }

        [Authorize(Roles = "USER")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var bookings = await _context.Bookings
                .Include(b => b.Bus)
                .ThenInclude(bus => bus!.Route)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    b.Status,
                    b.TotalAmount,
                    b.CreatedAt,
                    BusNumber = b.Bus!.BusNumber,
                    Source = b.Bus.Route!.Source,
                    Destination = b.Bus.Route.Destination
                })
                .ToListAsync();

            return Ok(new
            {
                Bookings = bookings,
                TotalSpent = bookings.Where(b => b.Status == BookingStatus.CONFIRMED).Sum(b => b.TotalAmount)
            });
        }

        [Authorize(Roles = "USER")]
        [HttpPut("profile")]
        public async Task<IActionResult> Profile([FromBody] UpdateProfileDto request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null) return NotFound();

            user.Name = request.Name;
            user.Phone = request.Phone;
            user.ProfileImage = request.ProfileImage;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated." });
        }
    }

    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
    }
}
