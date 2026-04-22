using System;

namespace BusBooking.Backend.DTOs
{
    public class AddBusRequestDto
    {
        public Guid RouteId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public string BusNumber { get; set; } = string.Empty;
    }

    public class AddRouteRequestDto
    {
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
    }
}
