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
    [Authorize(Roles = "ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BusBooking.Backend.Services.IEmailService _emailService;

        public AdminController(ApplicationDbContext context, BusBooking.Backend.Services.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("approve-operator/{id}")]
        public async Task<IActionResult> ApproveOperator(Guid id)
        {
            var op = await _context.Operators.FindAsync(id);
            if (op == null) return NotFound();

            op.Status = OperatorStatus.APPROVED;
            
            var user = await _context.Users.FindAsync(op.UserId);
            if (user != null) 
            {
                user.Role = UserRole.OPERATOR;
                try { await _emailService.SendEmailAsync(user.Email, "Operator Approved", "Your operator application has been approved!"); } catch { }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Operator approved." });
        }

        [HttpPost("add-route")]
        public async Task<IActionResult> AddRoute([FromBody] AddRouteRequestDto request)
        {
            var route = new BusBooking.Backend.Models.Route
            {
                Source = request.Source,
                Destination = request.Destination
            };
            
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Route added.", routeId = route.Id });
        }

        [HttpDelete("remove-bus/{id}")]
        public async Task<IActionResult> RemoveBus(Guid id)
        {
            var bus = await _context.Buses.Include(b => b.Operator).ThenInclude(o => o.User).FirstOrDefaultAsync(b => b.Id == id);
            if (bus == null) return NotFound();

            bus.Status = BusStatus.INACTIVE;
            await _context.SaveChangesAsync();

            if (bus.Operator?.User != null)
            {
                try { await _emailService.SendEmailAsync(bus.Operator.User.Email, "Bus Cancelled", $"Your bus {bus.BusNumber} has been marked inactive by the admin."); } catch { }
            }

            return Ok(new { message = "Bus marked as inactive." });
        }

        [HttpGet("operators")]
        public async Task<IActionResult> GetOperators()
        {
            var operators = await _context.Operators
                .Include(o => o.User)
                .OrderByDescending(o => o.Status == OperatorStatus.PENDING)
                .Select(o => new
                {
                    o.Id,
                    o.CompanyName,
                    o.Status,
                    o.UserId,
                    UserEmail = o.User != null ? o.User.Email : "No Email"
                })
                .ToListAsync();
            return Ok(operators);
        }

        [HttpGet("routes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _context.Routes.ToListAsync();
            return Ok(routes);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var totalRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.CONFIRMED)
                .SumAsync(b => b.TotalAmount);
                
            var activeBuses = await _context.Buses.CountAsync(b => b.Status == BusStatus.ACTIVE);
            
            var approvedOperators = await _context.Operators.CountAsync(o => o.Status == OperatorStatus.APPROVED);
            var pendingOperators = await _context.Operators.CountAsync(o => o.Status == OperatorStatus.PENDING);

            return Ok(new
            {
                TotalRevenue = totalRevenue,
                ActiveBuses = activeBuses,
                ApprovedOperators = approvedOperators,
                PendingOperators = pendingOperators
            });
        }
    }
}
