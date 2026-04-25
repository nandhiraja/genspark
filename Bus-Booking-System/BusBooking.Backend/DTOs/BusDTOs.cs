using System;
using System.ComponentModel.DataAnnotations;

namespace BusBooking.Backend.DTOs
{
    public class AddBusRequestDto
    {
        [Required]
        public Guid RouteId { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string BusNumber { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Range(typeof(decimal), "1", "100000")]
        public decimal Price { get; set; }

        [Range(1, 120)]
        public int TotalSeats { get; set; }

        public Guid? SourceBoardingPointId { get; set; }
        public Guid? DestinationBoardingPointId { get; set; }
    }


    public class AddRouteRequestDto
    {
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
    }
}
