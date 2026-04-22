using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class Seat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid BusId { get; set; }
        [ForeignKey("BusId")]
        public Bus? Bus { get; set; }
        
        public int SeatNumber { get; set; }
    }
}
