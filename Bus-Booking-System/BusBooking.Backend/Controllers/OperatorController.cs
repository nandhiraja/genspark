using BusBooking.Backend.DTOs;
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
    [Authorize(Roles = "OPERATOR")]
    public class OperatorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OperatorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("apply")]
        [Authorize(Roles = "USER")] // Users apply to become operators
        public async Task<IActionResult> Apply([FromBody] RegisterOperatorDto request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            
            var existingOp = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == userId);
            if (existingOp != null) return BadRequest("Already applied.");

            var operatorRecord = new Operator
            {
                UserId = userId,
                CompanyName = request.CompanyName,
                Status = OperatorStatus.PENDING
            };

            await _context.Operators.AddAsync(operatorRecord);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Application submitted." });
        }

        [HttpPost("add-bus")]
        public async Task<IActionResult> AddBus([FromBody] AddBusRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == userId);

            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid("Operator not approved.");

            var bus = new Bus
            {
                OperatorId = op.Id,
                RouteId = request.RouteId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Price = request.Price,
                TotalSeats = request.TotalSeats,
                AvailableSeats = request.TotalSeats,
                BusNumber = request.BusNumber,
                Status = BusStatus.ACTIVE
            };

            await _context.Buses.AddAsync(bus);
            await _context.SaveChangesAsync();

            // Auto-create seats
            for (int i = 1; i <= request.TotalSeats; i++)
            {
                await _context.Seats.AddAsync(new Seat { BusId = bus.Id, SeatNumber = i });
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bus added.", busId = bus.Id });
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> Bookings()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == userId);
            
            if (op == null) return Forbid();

            var bookings = await _context.Bookings
                .Include(b => b.Bus)
                .Where(b => b.Bus!.OperatorId == op.Id)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpGet("my-buses")]
        public async Task<IActionResult> MyBuses()
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == userId);
            if (op == null) return Forbid();

            var buses = await _context.Buses
                .Include(b => b.Route)
                .Where(b => b.OperatorId == op.Id)
                .Select(b => new
                {
                    b.Id,
                    b.BusNumber,
                    b.TotalSeats,
                    b.AvailableSeats,
                    b.Price,
                    b.Status,
                    b.StartTime,
                    b.EndTime,
                    Source = b.Route!.Source,
                    Destination = b.Route.Destination
                })
                .ToListAsync();

            return Ok(buses);
        }

        [HttpPut("bus-status/{busId}")]
        public async Task<IActionResult> UpdateBusStatus(Guid busId, [FromQuery] BusStatus status)
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == userId);
            
            if (op == null) return Forbid();

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == busId && b.OperatorId == op.Id);
            if (bus == null) return NotFound();

            bus.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bus status updated." });
        }
    }

    public class RegisterOperatorDto
    {
        public string CompanyName { get; set; } = string.Empty;
    }
}
