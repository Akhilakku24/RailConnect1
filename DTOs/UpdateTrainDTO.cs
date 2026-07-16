using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailwayReservation.DTOs
{
    public class UpdateTrainDTO
    {
        public string? Source { get; set; } = null!;
        public string? Destination { get; set; } = null!;
        public string? DepartureTime { get; set; } = null!;
        public string? ArrivalTime { get; set; } = null!;
        public decimal? BaseFare { get; set; }
        public int? TotalSeats { get; set; }
        public int? NumCoaches { get; set; }
        public decimal? BusinessPercentage { get; set; }

        public bool? IsActive { get; set; }
    }
}
