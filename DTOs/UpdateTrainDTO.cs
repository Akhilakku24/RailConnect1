using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RailwayReservation.DTOs
{
    public class UpdateTrainDTO
    {
        public string? Source { get; set; } = null!;
        public string? Destination { get; set; } = null!;
        public string? DepartureTime { get; set; } = null!;
        public string? ArrivalTime { get; set; } = null!;
        [Range(1,double.MaxValue)]
        public decimal? BaseFare { get; set; }
        [Range(1,int.MaxValue)]
        public int? TotalSeats { get; set; }
        [Range(1,int.MaxValue)]
        public int? NumCoaches { get; set; }
        [Range(typeof(decimal),"0","1")]
        public decimal? BusinessPercentage { get; set; }

        public bool? IsActive { get; set; }
    }
}
