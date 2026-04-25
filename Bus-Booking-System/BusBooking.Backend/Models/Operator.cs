using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class Operator
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        public OperatorStatus Status { get; set; } = OperatorStatus.PENDING;

        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Bus>? Buses { get; set; }   // ← was missing
    }
}