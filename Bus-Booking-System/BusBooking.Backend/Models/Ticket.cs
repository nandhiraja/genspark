using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class Ticket
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }
        
        public Guid SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int Age { get; set; }
        
        public string? Gender { get; set; }
        
        public string? Phone { get; set; }
    }
}
