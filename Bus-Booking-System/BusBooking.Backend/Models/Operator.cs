using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class Operator
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        public string? LogoUrl { get; set; }
        
        public OperatorStatus Status { get; set; } = OperatorStatus.PENDING;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
