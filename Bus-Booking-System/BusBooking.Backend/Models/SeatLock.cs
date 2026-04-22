using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class SeatLock
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid SeatId { get; set; }
        [ForeignKey("SeatId")]
        public Seat? Seat { get; set; }
        
        public Guid BusId { get; set; }
        [ForeignKey("BusId")]
        public Bus? Bus { get; set; }
        
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        public DateTime LockedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiryTime { get; set; }
    }
}
