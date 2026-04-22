using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class Booking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        public Guid BusId { get; set; }
        [ForeignKey("BusId")]
        public Bus? Bus { get; set; }
        
        public decimal TotalAmount { get; set; }
        
        public BookingStatus Status { get; set; } = BookingStatus.PENDING;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Ticket>? Tickets { get; set; }
    }
}
