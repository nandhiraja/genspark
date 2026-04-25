using System;
using System.Collections.Generic;

namespace BusBooking.Backend.DTOs
{
    public class LockSeatsRequestDto
    {
        public Guid BusId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }

    public class ConfirmBookingRequestDto
    {
        public Guid BusId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
        public List<PassengerDto> Passengers { get; set; } = new();
    }

    public class PassengerDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
    }
    public class UnlockSeatsRequestDto
    {
        public Guid BusId { get; set; }
        public List<Guid> SeatIds { get; set; } = new();
    }
}
