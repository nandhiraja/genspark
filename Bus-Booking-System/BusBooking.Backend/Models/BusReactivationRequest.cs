using System;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.Models
{
    public class BusReactivationRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BusId { get; set; }
        public Guid OperatorId { get; set; }

        [MaxLength(300)]
        public string? OperatorReason { get; set; }

        [MaxLength(300)]
        public string? AdminReason { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.PENDING;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public Bus? Bus { get; set; }
        public Operator? Operator { get; set; }
    }
}
