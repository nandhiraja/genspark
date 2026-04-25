using System;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class OperatorBoardingPoint
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OperatorId { get; set; }
        public Guid CityId { get; set; }

        [Required]
        [MaxLength(120)]
        public string Label { get; set; } = string.Empty;

        [Required]
        [MaxLength(300)]
        public string AddressLine { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Operator? Operator { get; set; }
        public City? City { get; set; }
    }
}
