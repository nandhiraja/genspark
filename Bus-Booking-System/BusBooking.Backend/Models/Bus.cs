using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class Bus
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid OperatorId { get; set; }
        [ForeignKey("OperatorId")]
        public Operator? Operator { get; set; }
        
        public Guid RouteId { get; set; }
        [ForeignKey("RouteId")]
        public Route? Route { get; set; }
        
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        
        public decimal Price { get; set; }
        
        [Required]
        public string BusNumber { get; set; } = string.Empty;
        
        public BusStatus Status { get; set; } = BusStatus.ACTIVE;
    }
}
