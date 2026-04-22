using System;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public UserRole Role { get; set; }
        
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? ProfileImage { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
