using System;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class Route
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Source { get; set; } = string.Empty;
        
        [Required]
        public string Destination { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
