using System;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class MockMailMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string FromEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string ToEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(240)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [MaxLength(4000)]
        public string Body { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }

        public Guid? ParentMessageId { get; set; }
        public MockMailMessage? ParentMessage { get; set; }
    }
}
