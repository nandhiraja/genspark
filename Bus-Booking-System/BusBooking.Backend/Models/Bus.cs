using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBooking.Backend.Models
{
    public class Bus
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string BusNumber { get; set; } = string.Empty;

        public Guid OperatorId { get; set; }
        public Guid RouteId { get; set; }
        public Guid? SourceBoardingPointId { get; set; }
        public Guid? DestinationBoardingPointId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public bool HiddenFromSearch { get; set; } = false;
        public bool AdminDeactivated { get; set; } = false;
        [MaxLength(300)]
        public string? AdminDeactivationReason { get; set; }

        public BusStatus Status { get; set; } = BusStatus.PENDING_APPROVAL;

        // Navigation properties
        public Operator? Operator { get; set; }
        public Route? Route { get; set; }
        public OperatorBoardingPoint? SourceBoardingPoint { get; set; }
        public OperatorBoardingPoint? DestinationBoardingPoint { get; set; }
        public ICollection<Seat>? Seats { get; set; }
        public ICollection<Booking>? Bookings { get; set; }   // ← was missing
        public ICollection<SeatLock>? SeatLocks { get; set; }
    }
}