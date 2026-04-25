using BusBooking.Backend.Models;
using BusBooking.Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
        private readonly IEmailService _emailService;
        private const decimal PlatformFeePercent = 5m;

        public AdminController(ApplicationDbContext context, IEmailService emailService)
        {
            _context      = context;
            _emailService = emailService;
        }

        // GET /api/admin/operators?status=PENDING
        [HttpGet("operators")]
        public async Task<IActionResult> GetOperators([FromQuery] string? status)
        {
            var query = _context.Operators.Include(o => o.User).AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OperatorStatus>(status, true, out var parsed))
                query = query.Where(o => o.Status == parsed);

            var operators = await query
                .OrderByDescending(o => o.Status == OperatorStatus.PENDING)
                .Select(o => new
                {
                    // Explicit operator row id for admin UI links (same as Id; avoids any client confusion with UserId).
                    OperatorId = o.Id,
                    o.Id,
                    o.CompanyName,
                    Status     = o.Status.ToString(),
                    o.UserId,
                    OwnerName  = o.User != null ? o.User.Name  : "Unknown",
                    UserEmail  = o.User != null ? o.User.Email : "No Email"
                })
                .ToListAsync();

            return Ok(operators);
        }

        // GET /api/admin/operator/{id}
        // GET /api/admin/operators/{id}
        // Note: use {id} without :guid so a bad segment returns 400 from our code, not an empty 404 from route constraints.
        [HttpGet("operator/{id}")]
        [HttpGet("operators/{id}")]
        public async Task<IActionResult> GetOperator(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                return BadRequest(new { message = "Invalid operator id format." });

            // Match by operator primary key or by linked user (owner) id.
            var op = await _context.Operators
                .AsNoTracking()
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == gid || o.UserId == gid);

            if (op == null) return NotFound(new { message = "Operator not found." });

            var buses = await _context.Buses
                .AsNoTracking()
                .Include(b => b.SourceBoardingPoint)
                .Include(b => b.DestinationBoardingPoint)
                .Where(b => b.OperatorId == op.Id)
                .OrderByDescending(b => b.StartTime)
                .Select(b => new
                {
                    b.Id,
                    b.BusNumber,
                    Status = b.Status.ToString(),
                    b.Price,
                    b.TotalSeats,
                    b.AvailableSeats,
                    b.HiddenFromSearch,
                    b.AdminDeactivated,
                    b.StartTime,
                    b.EndTime,
                    Source = b.Route != null ? b.Route.Source : null,
                    Destination = b.Route != null ? b.Route.Destination : null,
                    SourceBoardingPoint = b.SourceBoardingPoint != null ? b.SourceBoardingPoint.Label : null,
                    DestinationBoardingPoint = b.DestinationBoardingPoint != null ? b.DestinationBoardingPoint.Label : null
                })
                .ToListAsync();

            return Ok(new
            {
                op.Id,
                op.CompanyName,
                Status = op.Status.ToString(),
                op.CreatedAt,
                op.LogoUrl,
                Owner = op.User == null
                    ? null
                    : new { op.User.Id, op.User.Name, op.User.Email, Role = op.User.Role.ToString() },
                Buses = buses
            });
        }

        // PUT /api/admin/approve-operator/{id}
        [HttpPut("approve-operator/{id}")]
        public async Task<IActionResult> ApproveOperator(Guid id)
        {
            var op = await _context.Operators
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (op == null) return NotFound();
            if (op.Status == OperatorStatus.APPROVED)
                return BadRequest(new { message = "Already approved." });

            op.Status = OperatorStatus.APPROVED;

            if (op.User != null)
            {
                op.User.Role = UserRole.OPERATOR;
                _ = SendEmailSafe(op.User.Email,
                    "Operator Application Approved!",
                    $"Your operator application for '{op.CompanyName}' has been approved. You can now add buses.");
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Operator approved." });
        }

        // PUT /api/admin/reject-operator/{id}
        [HttpPut("reject-operator/{id}")]
        public async Task<IActionResult> RejectOperator(Guid id, [FromBody] AdminReasonDto? request)
        {
            var op = await _context.Operators
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (op == null) return NotFound();

            op.Status = OperatorStatus.REJECTED;
            await _context.SaveChangesAsync();

            if (op.User != null)
                _ = SendEmailSafe(op.User.Email,
                    "Operator Application Update",
                    $"Your application for '{op.CompanyName}' was not approved.\n" +
                    (request?.Reason != null ? $"Reason: {request.Reason}" : "Please contact support."));

            return Ok(new { message = "Operator rejected." });
        }

        // GET /api/admin/buses?status=PENDING_APPROVAL
        [HttpGet("buses")]
        public async Task<IActionResult> GetBuses([FromQuery] string? status)
        {
            var query = _context.Buses
                .Include(b => b.Route)
                .Include(b => b.Operator)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<BusStatus>(status, true, out var parsed))
                query = query.Where(b => b.Status == parsed);

            var buses = await query
                .OrderByDescending(b => b.Status == BusStatus.PENDING_APPROVAL)
                .Select(b => new
                {
                    b.Id, b.BusNumber, b.Price,
                    b.TotalSeats, b.AvailableSeats,
                    b.StartTime, b.EndTime,
                    Status       = b.Status.ToString(),
                    b.HiddenFromSearch,
                    b.AdminDeactivated,
                    OperatorName = b.Operator!.CompanyName,
                    Source       = b.Route!.Source,
                    Destination  = b.Route.Destination
                })
                .ToListAsync();

            return Ok(buses);
        }

        // PUT /api/admin/approve-bus/{id}
        [HttpPut("approve-bus/{id}")]
        public async Task<IActionResult> ApproveBus(Guid id)
        {
            var bus = await _context.Buses
                .Include(b => b.Operator).ThenInclude(o => o!.User)
                .Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bus == null) return NotFound();
            if (bus.Status == BusStatus.ACTIVE)
                return BadRequest(new { message = "Bus is already active." });

            bus.Status = BusStatus.ACTIVE;
            bus.HiddenFromSearch = false;
            bus.AdminDeactivated = false;
            bus.AdminDeactivationReason = null;
            await _context.SaveChangesAsync();

            var email = bus.Operator?.User?.Email;
            if (email != null)
                _ = SendEmailSafe(email,
                    "Your Bus Has Been Approved!",
                    $"Bus {bus.BusNumber} ({bus.Route?.Source} → {bus.Route?.Destination}) is now live.");

            return Ok(new { message = "Bus approved and is now active." });
        }

        // PUT /api/admin/bus/{id}/deactivate — same as remove-bus (email + reason); easier for JSON clients
        [HttpPut("bus/{id:guid}/deactivate")]
        public Task<IActionResult> DeactivateBus(Guid id, [FromBody] AdminReasonDto? request) =>
            RemoveBus(id, request);

        // DELETE /api/admin/remove-bus/{id}
        [HttpDelete("remove-bus/{id}")]
        public async Task<IActionResult> RemoveBus(Guid id, [FromBody] AdminReasonDto? request)
        {
            var bus = await _context.Buses
                .Include(b => b.Operator).ThenInclude(o => o!.User)
                .Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bus == null) return NotFound();

            bus.Status = BusStatus.INACTIVE;
            bus.AdminDeactivated = true;
            bus.HiddenFromSearch = true;
            bus.AdminDeactivationReason = string.IsNullOrWhiteSpace(request?.Reason) ? null : request!.Reason!.Trim();
            await _context.SaveChangesAsync();

            var email = bus.Operator?.User?.Email;
            if (email != null)
                _ = SendEmailSafe(email,
                    "Bus Removed by Admin",
                    $"Bus {bus.BusNumber} has been removed.\n" +
                    (request?.Reason != null ? $"Reason: {request.Reason}" : "Contact support for details."));

            return Ok(new { message = "Bus deactivated." });
        }

        // GET /api/admin/routes  (AllowAnonymous — used in operator + user dropdowns)
        [HttpGet("routes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _context.Routes
                .OrderBy(r => r.Source)
                .Select(r => new { r.Id, r.Source, r.Destination })
                .ToListAsync();
            return Ok(routes);
        }

        // POST /api/admin/add-route
        [HttpPost("add-route")]
        public async Task<IActionResult> AddRoute([FromBody] AddRouteRequestDto request)
        {
            var exists = await _context.Routes.AnyAsync(r =>
                r.Source.ToLower()      == request.Source.ToLower() &&
                r.Destination.ToLower() == request.Destination.ToLower());

            if (exists)
                return BadRequest(new { message = "This route already exists." });

            var route = new BusBooking.Backend.Models.Route { Source = request.Source, Destination = request.Destination };
            await _context.Routes.AddAsync(route);
            await EnsureCityAsync(request.Source);
            await EnsureCityAsync(request.Destination);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Route added.", routeId = route.Id });
        }

        // DELETE /api/admin/routes/{id}
        [HttpDelete("routes/{id:guid}")]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            var inUse = await _context.Buses.AnyAsync(b => b.RouteId == id);
            if (inUse)
                return BadRequest(new { message = "Cannot delete a route that still has buses. Remove or reassign those buses first." });

            var route = await _context.Routes.FindAsync(id);
            if (route == null)
                return NotFound(new { message = "Route not found." });

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Route deleted." });
        }

        // GET /api/admin/route-change-requests
        [HttpGet("route-change-requests")]
        public async Task<IActionResult> RouteChangeRequests([FromQuery] string? status)
        {
            var query = _context.BusRouteChangeRequests
                .AsNoTracking()
                .Include(r => r.Bus)
                .ThenInclude(b => b!.Route)
                .Include(r => r.Operator)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RequestStatus>(status, true, out var parsed))
                query = query.Where(r => r.Status == parsed);

            var rows = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.BusId,
                    r.OperatorId,
                    r.Status,
                    r.OperatorReason,
                    r.AdminReason,
                    r.CreatedAt,
                    r.ReviewedAt,
                    BusNumber = r.Bus!.BusNumber,
                    CurrentRoute = r.Bus.Route!.Source + " -> " + r.Bus.Route.Destination,
                    r.OldRouteId,
                    r.NewRouteId
                })
                .ToListAsync();
            return Ok(rows);
        }

        // PUT /api/admin/route-change-requests/{id}/approve
        [HttpPut("route-change-requests/{id:guid}/approve")]
        public async Task<IActionResult> ApproveRouteChange(Guid id)
        {
            var req = await _context.BusRouteChangeRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound(new { message = "Request not found." });
            if (req.Status != RequestStatus.PENDING)
                return BadRequest(new { message = "Request already reviewed." });

            var bus = await _context.Buses
                .Include(b => b.Route)
                .FirstOrDefaultAsync(b => b.Id == req.BusId);
            if (bus == null) return NotFound(new { message = "Bus not found." });

            var hasConfirmed = await _context.Bookings
                .AnyAsync(bk => bk.BusId == bus.Id && bk.Status == BookingStatus.CONFIRMED);
            if (hasConfirmed)
                return BadRequest(new { message = "Cannot approve route change; bus has confirmed bookings." });

            bus.RouteId = req.NewRouteId;
            bus.SourceBoardingPointId = req.NewSourceBoardingPointId;
            bus.DestinationBoardingPointId = req.NewDestinationBoardingPointId;
            bus.HiddenFromSearch = false;

            req.Status = RequestStatus.APPROVED;
            req.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Route change approved and applied." });
        }

        // PUT /api/admin/route-change-requests/{id}/reject
        [HttpPut("route-change-requests/{id:guid}/reject")]
        public async Task<IActionResult> RejectRouteChange(Guid id, [FromBody] AdminReasonDto? request)
        {
            var req = await _context.BusRouteChangeRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound(new { message = "Request not found." });
            if (req.Status != RequestStatus.PENDING)
                return BadRequest(new { message = "Request already reviewed." });

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == req.BusId);
            if (bus != null) bus.HiddenFromSearch = false;

            req.Status = RequestStatus.REJECTED;
            req.AdminReason = request?.Reason;
            req.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Route change request rejected." });
        }

        // GET /api/admin/reactivation-requests
        [HttpGet("reactivation-requests")]
        public async Task<IActionResult> ReactivationRequests([FromQuery] string? status)
        {
            var query = _context.BusReactivationRequests
                .AsNoTracking()
                .Include(r => r.Bus)
                .Include(r => r.Operator)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<RequestStatus>(status, true, out var parsed))
                query = query.Where(r => r.Status == parsed);

            var rows = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.BusId,
                    r.OperatorId,
                    r.Status,
                    r.OperatorReason,
                    r.AdminReason,
                    r.CreatedAt,
                    r.ReviewedAt,
                    BusNumber = r.Bus!.BusNumber
                })
                .ToListAsync();
            return Ok(rows);
        }

        // PUT /api/admin/reactivation-requests/{id}/approve
        [HttpPut("reactivation-requests/{id:guid}/approve")]
        public async Task<IActionResult> ApproveReactivation(Guid id)
        {
            var req = await _context.BusReactivationRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound(new { message = "Request not found." });
            if (req.Status != RequestStatus.PENDING)
                return BadRequest(new { message = "Request already reviewed." });

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == req.BusId);
            if (bus == null) return NotFound(new { message = "Bus not found." });

            bus.AdminDeactivated = false;
            bus.HiddenFromSearch = false;
            bus.Status = BusStatus.ACTIVE;
            bus.AdminDeactivationReason = null;
            req.Status = RequestStatus.APPROVED;
            req.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Reactivation approved." });
        }

        // PUT /api/admin/reactivation-requests/{id}/reject
        [HttpPut("reactivation-requests/{id:guid}/reject")]
        public async Task<IActionResult> RejectReactivation(Guid id, [FromBody] AdminReasonDto? request)
        {
            var req = await _context.BusReactivationRequests.FirstOrDefaultAsync(r => r.Id == id);
            if (req == null) return NotFound(new { message = "Request not found." });
            if (req.Status != RequestStatus.PENDING)
                return BadRequest(new { message = "Request already reviewed." });

            req.Status = RequestStatus.REJECTED;
            req.AdminReason = request?.Reason;
            req.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Reactivation rejected." });
        }

        // GET /api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var confirmedBookings = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Bus).ThenInclude(bus => bus!.Route)
                .Where(b => b.Status == BookingStatus.CONFIRMED)
                .ToListAsync();

            var totalBookingValue = confirmedBookings.Sum(b => b.TotalAmount);
            var platformRevenue = Math.Round(totalBookingValue * (PlatformFeePercent / (100 + PlatformFeePercent)), 2);
            var totalConfirmedBookings = confirmedBookings.Count;

            var activeBuses = await _context.Buses.CountAsync(b => b.Status == BusStatus.ACTIVE);
            var pendingBusApprovals = await _context.Buses.CountAsync(b => b.Status == BusStatus.PENDING_APPROVAL);
            var approvedOperators = await _context.Operators.CountAsync(o => o.Status == OperatorStatus.APPROVED);
            var pendingOperators = await _context.Operators.CountAsync(o => o.Status == OperatorStatus.PENDING);
            var registeredTravellers = await _context.Users.CountAsync(u => u.Role == UserRole.USER);

            var since30 = DateTime.UtcNow.AddDays(-30);
            var activeBookers30d = await _context.Bookings
                .Where(b => b.CreatedAt >= since30 && b.Status == BookingStatus.CONFIRMED)
                .Select(b => b.UserId)
                .Distinct()
                .CountAsync();

            var routeBreakdown = confirmedBookings
                .Where(b => b.Bus != null && b.Bus.Route != null)
                .GroupBy(b => new { b.Bus!.Route!.Source, b.Bus.Route.Destination })
                .Select(g => new
                {
                    g.Key.Source,
                    g.Key.Destination,
                    Revenue = g.Sum(x => x.TotalAmount),
                    Bookings = g.Count()
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();

            var operators = await _context.Operators
                .Include(o => o.Buses!).ThenInclude(b => b!.Bookings!)
                .Where(o => o.Status == OperatorStatus.APPROVED)
                .ToListAsync();

            var operatorBreakdown = operators.Select(o =>
            {
                var totalPaid = (o.Buses ?? Enumerable.Empty<Bus>())
                    .SelectMany(b => b.Bookings ?? Enumerable.Empty<Booking>())
                    .Where(bk => bk.Status == BookingStatus.CONFIRMED)
                    .Sum(bk => bk.TotalAmount);

                var opRevenue = Math.Round(totalPaid / (1 + PlatformFeePercent / 100), 2);

                return new
                {
                    o.Id,
                    o.CompanyName,
                    TotalRevenue = opRevenue,
                    TotalBookings = (o.Buses ?? Enumerable.Empty<Bus>())
                        .SelectMany(b => b.Bookings ?? Enumerable.Empty<Booking>())
                        .Count(bk => bk.Status == BookingStatus.CONFIRMED),
                    ActiveBuses = (o.Buses ?? Enumerable.Empty<Bus>())
                        .Count(b => b.Status == BusStatus.ACTIVE)
                };
            })
            .OrderByDescending(o => o.TotalRevenue)
            .ToList();

            return Ok(new
            {
                TotalBookingValue = totalBookingValue,
                PlatformRevenue = platformRevenue,
                ActiveBuses = activeBuses,
                PendingBusApprovals = pendingBusApprovals,
                ApprovedOperators = approvedOperators,
                PendingOperators = pendingOperators,
                TotalConfirmedBookings = totalConfirmedBookings,
                RegisteredTravellers = registeredTravellers,
                ActiveBookersLast30Days = activeBookers30d,
                OperatorBreakdown = operatorBreakdown,
                RouteBreakdown = routeBreakdown
            });
        }

        private async Task SendEmailSafe(string email, string subject, string body)
        {
            try { await _emailService.SendEmailAsync(email, subject, body); }
            catch { }
        }

        private async Task EnsureCityAsync(string cityName)
        {
            var trimmed = cityName.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) return;
            var exists = await _context.Cities.AnyAsync(c => c.Name.ToLower() == trimmed.ToLower());
            if (!exists) _context.Cities.Add(new City { Name = trimmed });
        }
    }

    public class AdminReasonDto  { public string? Reason { get; set; } }
    public class AddRouteRequestDto
    {
        public string Source      { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
    }
}
