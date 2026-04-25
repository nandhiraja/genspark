using BusBooking.Backend.DTOs;
using BusBooking.Backend.Models;
using BusBooking.Backend.Services;
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
    [Authorize]
    public class OperatorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public OperatorController(ApplicationDbContext context, IEmailService emailService)
        {
            _context      = context;
            _emailService = emailService;
        }

        private bool TryGetCurrentUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var raw = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return raw != null && Guid.TryParse(raw, out userId);
        }

        // POST /api/operator/apply
        [HttpPost("apply")]
        [Authorize(Roles = "USER")]
        public async Task<IActionResult> Apply([FromBody] RegisterOperatorDto request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });

            var existing = await _context.Operators
                .FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (existing != null)
                return BadRequest(new { message = "You have already applied." });

            await _context.Operators.AddAsync(new Operator
            {
                UserId      = currentUserId,
                CompanyName = request.CompanyName,
                Status      = OperatorStatus.PENDING
            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "Application submitted. An admin will review it." });
        }

        // GET /api/operator/my-status
        [HttpGet("my-status")]
        public async Task<IActionResult> MyStatus()
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });

            var op = await _context.Operators
                .FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null)
                return Ok(new { status = (string?)null });

            var userRole = await _context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.Role.ToString())
                .FirstOrDefaultAsync();

            return Ok(new { status = op.Status.ToString(), companyName = op.CompanyName, role = userRole });
        }

        // GET /api/operator/cities
        [HttpGet("cities")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> Cities()
        {
            // Avoid SelectMany(array) translation issues on some EF/Npgsql combinations.
            var sourceNames = await _context.Routes
                .Select(r => r.Source)
                .ToListAsync();
            var destinationNames = await _context.Routes
                .Select(r => r.Destination)
                .ToListAsync();
            var names = sourceNames
                .Concat(destinationNames)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
            return Ok(names.Select(n => new { name = n }));
        }

        // GET /api/operator/boarding-points?city=Chennai
        [HttpGet("boarding-points")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> BoardingPoints([FromQuery] string? city)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });

            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var query = _context.OperatorBoardingPoints
                .AsNoTracking()
                .Include(p => p.City)
                .Where(p => p.OperatorId == op.Id && p.IsActive);

            if (!string.IsNullOrWhiteSpace(city))
            {
                var cityNorm = city.Trim().ToLower();
                query = query.Where(p => p.City != null && p.City.Name.ToLower() == cityNorm);
            }

            var points = await query
                .OrderBy(p => p.City!.Name)
                .ThenBy(p => p.Label)
                .Select(p => new
                {
                    p.Id,
                    p.Label,
                    p.AddressLine,
                    City = p.City!.Name
                })
                .ToListAsync();

            return Ok(points);
        }

        // POST /api/operator/boarding-points
        [HttpPost("boarding-points")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> AddBoardingPoint([FromBody] UpsertBoardingPointDto request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var city = await EnsureCityAsync(request.City);

            var exists = await _context.OperatorBoardingPoints.AnyAsync(p =>
                p.OperatorId == op.Id &&
                p.CityId == city.Id &&
                p.Label.ToLower() == request.Label.ToLower());
            if (exists)
                return BadRequest(new { message = "Boarding point label already exists for this city." });

            var point = new OperatorBoardingPoint
            {
                OperatorId = op.Id,
                CityId = city.Id,
                Label = request.Label.Trim(),
                AddressLine = request.AddressLine.Trim(),
                IsActive = true
            };
            _context.OperatorBoardingPoints.Add(point);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Boarding point added.", pointId = point.Id });
        }

        // PUT /api/operator/boarding-points/{id}
        [HttpPut("boarding-points/{id:guid}")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> UpdateBoardingPoint(Guid id, [FromBody] UpsertBoardingPointDto request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var point = await _context.OperatorBoardingPoints
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.Id == id && p.OperatorId == op.Id);
            if (point == null) return NotFound(new { message = "Boarding point not found." });

            var city = await EnsureCityAsync(request.City);
            point.CityId = city.Id;
            point.Label = request.Label.Trim();
            point.AddressLine = request.AddressLine.Trim();
            point.IsActive = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Boarding point updated." });
        }

        // DELETE /api/operator/boarding-points/{id}
        [HttpDelete("boarding-points/{id:guid}")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> DeleteBoardingPoint(Guid id)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var point = await _context.OperatorBoardingPoints
                .FirstOrDefaultAsync(p => p.Id == id && p.OperatorId == op.Id);
            if (point == null) return NotFound(new { message = "Boarding point not found." });

            var inUse = await _context.Buses.AnyAsync(b =>
                b.SourceBoardingPointId == id || b.DestinationBoardingPointId == id);
            if (inUse)
                return BadRequest(new { message = "Boarding point is used by a bus and cannot be removed." });

            point.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Boarding point removed." });
        }

        // POST /api/operator/add-bus
        [HttpPost("add-bus")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> AddBus([FromBody] AddBusRequestDto request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });

            var op = await _context.Operators
                .FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var startUtc = NormalizeToUtc(request.StartTime);
            var endUtc = NormalizeToUtc(request.EndTime);

            if (startUtc >= endUtc)
                return BadRequest(new { message = "End time must be after start time." });
            if (startUtc <= DateTime.UtcNow)
                return BadRequest(new { message = "Start time must be in the future." });
            if (request.TotalSeats < 1)
                return BadRequest(new { message = "Total seats must be at least 1." });
            if (request.Price <= 0)
                return BadRequest(new { message = "Price must be greater than zero." });

            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == request.RouteId);
            if (route == null)
                return BadRequest(new { message = "Selected route does not exist." });

            var busNumberNorm = request.BusNumber.Trim().ToUpperInvariant();
            var duplicate = await _context.Buses.AnyAsync(b => b.BusNumber.ToUpper() == busNumberNorm);
            if (duplicate)
                return Conflict(new { message = "Bus number already exists." });

            Guid? sourcePointId = request.SourceBoardingPointId;
            Guid? destinationPointId = request.DestinationBoardingPointId;
            if (sourcePointId.HasValue || destinationPointId.HasValue)
            {
                if (!sourcePointId.HasValue || !destinationPointId.HasValue)
                    return BadRequest(new { message = "Both source and destination boarding points are required together." });

                var points = await _context.OperatorBoardingPoints
                    .Include(p => p.City)
                    .Where(p => p.OperatorId == op.Id &&
                                (p.Id == sourcePointId.Value || p.Id == destinationPointId.Value) &&
                                p.IsActive)
                    .ToListAsync();
                var src = points.FirstOrDefault(p => p.Id == sourcePointId.Value);
                var dst = points.FirstOrDefault(p => p.Id == destinationPointId.Value);
                if (src == null || dst == null)
                    return BadRequest(new { message = "Invalid boarding point selection." });
                if (!string.Equals(src.City?.Name, route.Source, StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { message = "Source boarding point city must match route source." });
                if (!string.Equals(dst.City?.Name, route.Destination, StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { message = "Destination boarding point city must match route destination." });
            }

            var bus = new Bus
            {
                OperatorId     = op.Id,
                RouteId        = request.RouteId,
                SourceBoardingPointId = sourcePointId,
                DestinationBoardingPointId = destinationPointId,
                StartTime      = startUtc,
                EndTime        = endUtc,
                Price          = request.Price,
                TotalSeats     = request.TotalSeats,
                AvailableSeats = request.TotalSeats,
                BusNumber      = busNumberNorm,
                Status         = BusStatus.PENDING_APPROVAL
            };

            try
            {
                await _context.Buses.AddAsync(bus);
                await _context.SaveChangesAsync();
                for (int i = 1; i <= request.TotalSeats; i++)
                    await _context.Seats.AddAsync(new Seat { BusId = bus.Id, SeatNumber = i });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Could not save bus. Check route and bus number uniqueness." });
            }

            return Ok(new { message = "Bus submitted for admin approval.", busId = bus.Id });
        }

        // GET /api/operator/my-buses
        [HttpGet("my-buses")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> MyBuses()
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null) return Forbid();

            var buses = await _context.Buses
                .Include(b => b.Route)
                .Include(b => b.SourceBoardingPoint).ThenInclude(p => p!.City)
                .Include(b => b.DestinationBoardingPoint).ThenInclude(p => p!.City)
                .Where(b => b.OperatorId == op.Id)
                .Select(b => new
                {
                    b.Id, b.BusNumber, b.TotalSeats, b.AvailableSeats,
                    b.Price, b.StartTime, b.EndTime,
                    Status      = b.Status.ToString(),
                    b.AdminDeactivated,
                    b.AdminDeactivationReason,
                    b.HiddenFromSearch,
                    Source      = b.Route!.Source,
                    Destination = b.Route.Destination,
                    ReactivationRequest = _context.BusReactivationRequests
                        .Where(r => r.BusId == b.Id)
                        .OrderByDescending(r => r.CreatedAt)
                        .Select(r => new
                        {
                            Status = r.Status.ToString(),
                            r.OperatorReason,
                            r.AdminReason,
                            r.CreatedAt,
                            r.ReviewedAt
                        })
                        .FirstOrDefault(),
                    SourceBoardingPoint = b.SourceBoardingPoint == null ? null : new
                    {
                        b.SourceBoardingPoint.Id,
                        b.SourceBoardingPoint.Label,
                        b.SourceBoardingPoint.AddressLine,
                        City = b.SourceBoardingPoint.City!.Name
                    },
                    DestinationBoardingPoint = b.DestinationBoardingPoint == null ? null : new
                    {
                        b.DestinationBoardingPoint.Id,
                        b.DestinationBoardingPoint.Label,
                        b.DestinationBoardingPoint.AddressLine,
                        City = b.DestinationBoardingPoint.City!.Name
                    }
                })
                .ToListAsync();

            return Ok(buses);
        }

        // GET /api/operator/bookings
        [HttpGet("bookings")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> Bookings()
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null) return Forbid();

            var bookings = await _context.Bookings
                .Include(b => b.Bus).ThenInclude(bus => bus!.Route)
                .Include(b => b.Tickets)
                .Where(b => b.Bus!.OperatorId == op.Id && b.Status == BookingStatus.CONFIRMED)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id, b.TotalAmount, b.CreatedAt,
                    Status         = b.Status.ToString(),
                    BusNumber      = b.Bus!.BusNumber,
                    Source         = b.Bus.Route!.Source,
                    Destination    = b.Bus.Route.Destination,
                    PassengerCount = b.Tickets!.Count
                })
                .ToListAsync();

            return Ok(bookings);
        }

        // GET /api/operator/revenue
        [HttpGet("revenue")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> Revenue()
        {
            const decimal PlatformFeePercent = 5m;
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null) return Forbid();

            var buses = await _context.Buses
                .Include(b => b.Route)
                .Include(b => b.Bookings)
                .Where(b => b.OperatorId == op.Id)
                .ToListAsync();

            var busBreakdown = buses.Select(b =>
            {
                var totalPaid = b.Bookings?
                    .Where(bk => bk.Status == BookingStatus.CONFIRMED)
                    .Sum(bk => bk.TotalAmount) ?? 0;

                var opRevenue = Math.Round(totalPaid / (1 + PlatformFeePercent / 100), 2);

                return new
                {
                    b.Id,
                    b.BusNumber,
                    Source        = b.Route?.Source ?? "",
                    Destination   = b.Route?.Destination ?? "",
                    Status        = b.Status.ToString(),
                    Revenue       = opRevenue, // Operator's portion
                    TotalBookings = b.Bookings?
                        .Count(bk => bk.Status == BookingStatus.CONFIRMED) ?? 0
                };
            }).ToList();

            return Ok(new
            {
                TotalRevenue = busBreakdown.Sum(b => b.Revenue),
                ActiveBuses  = buses.Count(b => b.Status == BusStatus.ACTIVE),
                PendingBuses = buses.Count(b => b.Status == BusStatus.PENDING_APPROVAL),
                HiddenBuses = buses.Count(b => b.HiddenFromSearch),
                AdminDeactivatedBuses = buses.Count(b => b.AdminDeactivated),
                BusBreakdown = busBreakdown
            });
        }

        // PUT /api/operator/bus-status/{busId}?status=INACTIVE
        [HttpPut("bus-status/{busId}")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> UpdateBusStatus(Guid busId, [FromQuery] BusStatus status)
        {
            if (status == BusStatus.PENDING_APPROVAL)
                return BadRequest(new { message = "Invalid status." });

            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null) return Forbid();

            var bus = await _context.Buses
                .FirstOrDefaultAsync(b => b.Id == busId && b.OperatorId == op.Id);
            if (bus == null) return NotFound();

            if (bus.AdminDeactivated && status == BusStatus.ACTIVE)
                return BadRequest(new { message = "This bus was deactivated by admin. Submit a reactivation request." });

            bus.Status = status;
            if (status != BusStatus.ACTIVE) bus.HiddenFromSearch = true;
            if (status == BusStatus.ACTIVE && !bus.AdminDeactivated) bus.HiddenFromSearch = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Bus status updated." });
        }

        // PUT /api/operator/bus/{busId}/route-change
        [HttpPut("bus/{busId:guid}/route-change")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> RequestRouteChange(Guid busId, [FromBody] RouteChangeRequestDto request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == busId && b.OperatorId == op.Id);
            if (bus == null) return NotFound(new { message = "Bus not found." });
            if (bus.AdminDeactivated)
                return BadRequest(new { message = "Bus is admin-deactivated. Request reactivation first." });

            var hasConfirmed = await _context.Bookings.AnyAsync(bk => bk.BusId == bus.Id && bk.Status == BookingStatus.CONFIRMED);
            if (hasConfirmed)
                return BadRequest(new { message = "Cannot change route because confirmed bookings already exist." });

            var pendingExists = await _context.BusRouteChangeRequests
                .AnyAsync(r => r.BusId == bus.Id && r.Status == RequestStatus.PENDING);
            if (pendingExists)
                return BadRequest(new { message = "A route-change request is already pending for this bus." });

            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == request.NewRouteId);
            if (route == null)
                return BadRequest(new { message = "Selected new route does not exist." });

            if (request.NewSourceBoardingPointId.HasValue || request.NewDestinationBoardingPointId.HasValue)
            {
                if (!request.NewSourceBoardingPointId.HasValue || !request.NewDestinationBoardingPointId.HasValue)
                    return BadRequest(new { message = "Select both source and destination boarding points." });
            }

            var req = new BusRouteChangeRequest
            {
                BusId = bus.Id,
                OperatorId = op.Id,
                OldRouteId = bus.RouteId,
                NewRouteId = route.Id,
                OldSourceBoardingPointId = bus.SourceBoardingPointId,
                OldDestinationBoardingPointId = bus.DestinationBoardingPointId,
                NewSourceBoardingPointId = request.NewSourceBoardingPointId,
                NewDestinationBoardingPointId = request.NewDestinationBoardingPointId,
                OperatorReason = request.Reason,
                Status = RequestStatus.PENDING
            };
            _context.BusRouteChangeRequests.Add(req);

            bus.HiddenFromSearch = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Route-change request sent to admin. Bus is hidden from user search until decision." });
        }

        // POST /api/operator/bus/{busId}/reactivation-request
        [HttpPost("bus/{busId:guid}/reactivation-request")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> RequestReactivation(Guid busId, [FromBody] OperatorReasonDto? request)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == busId && b.OperatorId == op.Id);
            if (bus == null) return NotFound(new { message = "Bus not found." });
            if (!bus.AdminDeactivated)
                return BadRequest(new { message = "Only admin-deactivated buses can request reactivation." });

            var pendingExists = await _context.BusReactivationRequests
                .AnyAsync(r => r.BusId == bus.Id && r.Status == RequestStatus.PENDING);
            if (pendingExists)
                return BadRequest(new { message = "A reactivation request is already pending." });

            _context.BusReactivationRequests.Add(new BusReactivationRequest
            {
                BusId = bus.Id,
                OperatorId = op.Id,
                OperatorReason = request?.Reason,
                Status = RequestStatus.PENDING
            });
            await _context.SaveChangesAsync();
            return Ok(new { message = "Reactivation request submitted to admin." });
        }

        // DELETE /api/operator/bus/{busId}
        [HttpDelete("bus/{busId:guid}")]
        [Authorize(Roles = "OPERATOR")]
        public async Task<IActionResult> DeleteBus(Guid busId)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
                return Unauthorized(new { message = "Invalid token claims." });
            var op = await _context.Operators.FirstOrDefaultAsync(o => o.UserId == currentUserId);
            if (op == null || op.Status != OperatorStatus.APPROVED)
                return Forbid();

            var bus = await _context.Buses.FirstOrDefaultAsync(b => b.Id == busId && b.OperatorId == op.Id);
            if (bus == null) return NotFound(new { message = "Bus not found." });

            var hasBookings = await _context.Bookings.AnyAsync(bk => bk.BusId == bus.Id);
            if (hasBookings)
                return BadRequest(new { message = "Cannot delete bus with booking history. Set it inactive instead." });

            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Bus deleted successfully." });
        }

        private async Task<City> EnsureCityAsync(string cityName)
        {
            var trimmed = cityName.Trim();
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == trimmed.ToLower());
            if (city != null) return city;
            city = new City { Name = trimmed };
            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return city;
        }

        private static DateTime NormalizeToUtc(DateTime value)
        {
            if (value.Kind == DateTimeKind.Utc) return value;
            if (value.Kind == DateTimeKind.Local) return value.ToUniversalTime();
            return DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime();
        }

        private async Task SendEmailSafe(string email, string subject, string body)
        {
            try { await _emailService.SendEmailAsync(email, subject, body); }
            catch { }
        }
    }

    public class RegisterOperatorDto
    {
        public string CompanyName { get; set; } = string.Empty;
    }

    public class UpsertBoardingPointDto
    {
        public string City { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
    }

    public class RouteChangeRequestDto
    {
        public Guid NewRouteId { get; set; }
        public Guid? NewSourceBoardingPointId { get; set; }
        public Guid? NewDestinationBoardingPointId { get; set; }
        public string? Reason { get; set; }
    }

    public class OperatorReasonDto
    {
        public string? Reason { get; set; }
    }
}