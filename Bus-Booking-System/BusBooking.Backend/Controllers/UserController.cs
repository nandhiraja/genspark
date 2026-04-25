using BusBooking.Backend.Models;
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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const decimal PlatformFeePercent = 5m;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

        private static string BookingStatusForApi(BookingStatus s) => s.ToString().ToUpperInvariant();

        // GET /api/user/locations
        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
        {
            var sources = await _context.Routes
                .Select(r => r.Source)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var destinations = await _context.Routes
                .Select(r => r.Destination)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return Ok(new { sources, destinations });
        }

        // GET /api/user/search-buses
        [HttpGet("search-buses")]
        public async Task<IActionResult> SearchBuses(
            [FromQuery] string? source,
            [FromQuery] string? destination,
            [FromQuery] string? date)
        {
            Console.WriteLine($"DEBUG: SearchBuses hit. Source: {source}, Dest: {destination}, Date: {date}");
            
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d))
            {
                parsedDate = DateTime.SpecifyKind(d.Date, DateTimeKind.Utc);
            }

            var query = _context.Buses
                .Include(b => b.Route)
                .Include(b => b.Operator)
                .Include(b => b.SourceBoardingPoint)
                .Include(b => b.DestinationBoardingPoint)
                .Where(b => b.Status == BusStatus.ACTIVE && !b.HiddenFromSearch && !b.AdminDeactivated);

            if (!string.IsNullOrEmpty(source))
                query = query.Where(b => b.Route!.Source.ToLower() == source.ToLower());

            if (!string.IsNullOrEmpty(destination))
                query = query.Where(b => b.Route!.Destination.ToLower() == destination.ToLower());

            if (parsedDate.HasValue)
            {
                var nextDay = parsedDate.Value.AddDays(1);
                query = query.Where(b => b.StartTime >= parsedDate.Value && b.StartTime < nextDay);
            }
            else
            {
                query = query.Where(b => b.StartTime >= DateTime.UtcNow);
            }

            var busRows = await query
                .OrderBy(b => b.StartTime)
                .Select(b => new
                {
                    b.Id,
                    b.BusNumber,
                    b.StartTime,
                    b.EndTime,
                    BasePrice    = b.Price,
                    PlatformFee  = Math.Round(b.Price * PlatformFeePercent / 100, 2),
                    TotalPrice   = Math.Round(b.Price * (1 + PlatformFeePercent / 100), 2),
                    b.TotalSeats,
                    OperatorName = b.Operator!.CompanyName,
                    Source       = b.Route!.Source,
                    Destination  = b.Route.Destination,
                    SourceBoardingPoint = b.SourceBoardingPoint == null ? null : new
                    {
                        b.SourceBoardingPoint.Label,
                        b.SourceBoardingPoint.AddressLine
                    },
                    DestinationBoardingPoint = b.DestinationBoardingPoint == null ? null : new
                    {
                        b.DestinationBoardingPoint.Label,
                        b.DestinationBoardingPoint.AddressLine
                    }
                })
                .ToListAsync();

            // Fallback: If no buses found for the specific date, but route matches, show all upcoming for that route
            if (busRows.Count == 0 && !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(destination) && parsedDate.HasValue)
            {
                busRows = await _context.Buses
                    .Include(b => b.Route)
                    .Include(b => b.Operator)
                    .Include(b => b.SourceBoardingPoint)
                    .Include(b => b.DestinationBoardingPoint)
                    .Where(b => b.Route!.Source.ToLower() == source.ToLower() 
                             && b.Route.Destination.ToLower() == destination.ToLower()
                             && b.Status == BusStatus.ACTIVE
                             && !b.HiddenFromSearch
                             && !b.AdminDeactivated
                             && b.StartTime >= DateTime.UtcNow)
                    .OrderBy(b => b.StartTime)
                    .Select(b => new
                    {
                        b.Id,
                        b.BusNumber,
                        b.StartTime,
                        b.EndTime,
                        BasePrice    = b.Price,
                        PlatformFee  = Math.Round(b.Price * PlatformFeePercent / 100, 2),
                        TotalPrice   = Math.Round(b.Price * (1 + PlatformFeePercent / 100), 2),
                        b.TotalSeats,
                        OperatorName = b.Operator!.CompanyName,
                        Source       = b.Route!.Source,
                        Destination  = b.Route.Destination,
                        SourceBoardingPoint = b.SourceBoardingPoint == null ? null : new
                        {
                            b.SourceBoardingPoint.Label,
                            b.SourceBoardingPoint.AddressLine
                        },
                        DestinationBoardingPoint = b.DestinationBoardingPoint == null ? null : new
                        {
                            b.DestinationBoardingPoint.Label,
                            b.DestinationBoardingPoint.AddressLine
                        }
                    })
                    .ToListAsync();
            }

            var busIds = busRows.Select(x => x.Id).ToList();
            var bookedByBus = await _context.Tickets
                .Where(t => t.Booking!.Status == BookingStatus.CONFIRMED && busIds.Contains(t.Seat!.BusId))
                .GroupBy(t => t.Seat!.BusId)
                .Select(g => new { BusId = g.Key, Booked = g.Count() })
                .ToListAsync();
            var bookedMap = bookedByBus.ToDictionary(x => x.BusId, x => x.Booked);

            var buses = busRows.Select(b => new
            {
                b.Id,
                b.BusNumber,
                b.StartTime,
                b.EndTime,
                b.BasePrice,
                b.PlatformFee,
                b.TotalPrice,
                b.TotalSeats,
                AvailableSeats = Math.Max(0, b.TotalSeats - bookedMap.GetValueOrDefault(b.Id, 0)),
                b.OperatorName,
                b.Source,
                b.Destination,
                b.SourceBoardingPoint,
                b.DestinationBoardingPoint
            }).ToList();

            return Ok(buses);
        }

        // GET /api/user/bus/{busId}
        [HttpGet("bus/{busId}")]
        public async Task<IActionResult> GetBusDetail(Guid busId)
        {
            var bus = await _context.Buses
                .Include(b => b.Route)
                .Include(b => b.Operator)
                .Include(b => b.SourceBoardingPoint)
                .Include(b => b.DestinationBoardingPoint)
                .FirstOrDefaultAsync(b => b.Id == busId && b.Status == BusStatus.ACTIVE && !b.HiddenFromSearch && !b.AdminDeactivated);

            if (bus == null)
                return NotFound(new { message = "Bus not found or not available." });

            var seats = await _context.Seats
                .Where(s => s.BusId == busId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();

            // FIX: ToHashSetAsync does not exist — use ToListAsync then .ToHashSet()
            var bookedSeatIds = new HashSet<Guid>(
                await _context.Tickets
                    .Include(t => t.Booking)
                    .Where(t => t.Seat!.BusId == busId
                             && t.Booking!.Status == BookingStatus.CONFIRMED)
                    .Select(t => t.SeatId)
                    .ToListAsync()
            );

            var lockedSeatIds = new HashSet<Guid>(
                await _context.SeatLocks
                    .Where(sl => sl.BusId == busId && sl.ExpiryTime > DateTime.UtcNow)
                    .Select(sl => sl.SeatId)
                    .ToListAsync()
            );

            var bookedCount = bookedSeatIds.Count;

            var seatLayout = seats.Select(s => new
            {
                s.Id,
                s.SeatNumber,
                Status = bookedSeatIds.Contains(s.Id) ? "BOOKED"
                       : lockedSeatIds.Contains(s.Id) ? "LOCKED"
                       : "AVAILABLE"
            }).ToList();

            var avail = Math.Max(0, bus.TotalSeats - bookedCount);

            return Ok(new
            {
                bus.Id,
                bus.BusNumber,
                Source        = bus.Route!.Source,
                Destination   = bus.Route.Destination,
                DepartureTime = bus.StartTime,
                ArrivalTime   = bus.EndTime,
                BasePrice     = bus.Price,
                PlatformFee   = Math.Round(bus.Price * PlatformFeePercent / 100, 2),
                TotalPrice    = Math.Round(bus.Price * (1 + PlatformFeePercent / 100), 2),
                bus.TotalSeats,
                AvailableSeats = avail,
                OperatorName  = bus.Operator!.CompanyName,
                SourceBoardingPoint = bus.SourceBoardingPoint == null ? null : new
                {
                    bus.SourceBoardingPoint.Label,
                    bus.SourceBoardingPoint.AddressLine
                },
                DestinationBoardingPoint = bus.DestinationBoardingPoint == null ? null : new
                {
                    bus.DestinationBoardingPoint.Label,
                    bus.DestinationBoardingPoint.AddressLine
                },
                SeatLayout    = seatLayout
            });
        }

        // GET /api/user/bus-seats/{busId}
        [HttpGet("bus-seats/{busId}")]
        public async Task<IActionResult> BusSeats(Guid busId)
        {
            var seats = await _context.Seats
                .Where(s => s.BusId == busId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();

            var bookedSeatIds = new HashSet<Guid>(
                await _context.Tickets
                    .Include(t => t.Booking)
                    .Where(t => t.Seat!.BusId == busId
                             && t.Booking!.Status == BookingStatus.CONFIRMED)
                    .Select(t => t.SeatId)
                    .ToListAsync()
            );

            var lockedSeatIds = new HashSet<Guid>(
                await _context.SeatLocks
                    .Where(sl => sl.BusId == busId && sl.ExpiryTime > DateTime.UtcNow)
                    .Select(sl => sl.SeatId)
                    .ToListAsync()
            );

            var result = seats.Select(s => new
            {
                s.Id,
                s.SeatNumber,
                Status = bookedSeatIds.Contains(s.Id) ? "BOOKED"
                       : lockedSeatIds.Contains(s.Id) ? "LOCKED"
                       : "AVAILABLE"
            });

            return Ok(result);
        }

        // GET /api/user/dashboard
        [Authorize(Roles = "USER")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = CurrentUserId;
            var rows = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Bus!).ThenInclude(bus => bus!.Route)
                .Include(b => b.Tickets!).ThenInclude(t => t.Seat)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            var bookings = rows.Select(b => new
            {
                b.Id,
                Status = BookingStatusForApi(b.Status),
                b.TotalAmount,
                b.CreatedAt,
                BusNumber = b.Bus!.BusNumber,
                Source = b.Bus.Route!.Source,
                Destination = b.Bus.Route.Destination,
                DepartureTime = b.Bus.StartTime,
                SeatNumbers = (b.Tickets ?? new List<Ticket>())
                    .OrderBy(t => t.Seat!.SeatNumber)
                    .Select(t => t.Seat!.SeatNumber)
                    .ToList(),
                PassengerNames = (b.Tickets ?? new List<Ticket>())
                    .OrderBy(t => t.Seat!.SeatNumber)
                    .Select(t => t.Name)
                    .ToList()
            }).ToList();

            var now = DateTime.UtcNow;

            var totalSpent = rows
                .Where(b => b.Status == BookingStatus.CONFIRMED)
                .Sum(b => b.TotalAmount);

            var upcomingTrips = bookings
                .Where(b => string.Equals(b.Status, "CONFIRMED", StringComparison.OrdinalIgnoreCase)
                            && b.DepartureTime > now)
                .ToList();

            var pastTrips = bookings
                .Where(b => string.Equals(b.Status, "CONFIRMED", StringComparison.OrdinalIgnoreCase)
                            && b.DepartureTime <= now)
                .ToList();

            return Ok(new
            {
                AllBookings   = bookings,
                UpcomingTrips = upcomingTrips,
                PastTrips     = pastTrips,
                TotalSpent    = totalSpent
            });
        }

        // PUT /api/user/profile
        [Authorize(Roles = "USER")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var user = await _context.Users.FindAsync(CurrentUserId);
            if (user == null) return NotFound();
            if (request.Name != null)         user.Name         = request.Name;
            if (request.Phone != null)        user.Phone        = request.Phone;
            if (request.ProfileImage != null) user.ProfileImage = request.ProfileImage;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated." });
        }
    }

    public class UpdateProfileDto
    {
        public string? Name         { get; set; }
        public string? Phone        { get; set; }
        public string? ProfileImage { get; set; }
    }
}