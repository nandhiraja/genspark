using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class City
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OperatorBoardingPoint>? BoardingPoints { get; set; }
    }
}
