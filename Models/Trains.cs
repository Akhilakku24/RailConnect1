using System.ComponentModel.DataAnnotations;

namespace RailwayReservation.Models
{
    public class Train
    {
        [Key]
        public int TrainId { get; set; }
        [Required]
        public string TrainNo { get; set; } = null!;
        [Required]
        public string Source { get; set; } = null!;
        [Required]
        public string Destination { get; set; } = null!;
        [Required]
        public string DepartureTime { get; set; } = null!;
        [Required]
        public string ArrivalTime { get; set; } = null!;
        [Range(1, double.MaxValue)]
        public decimal BaseFare { get; set; }
        [Range(1, int.MaxValue)]
        public int TotalSeats { get; set; }
        [Range(1, int.MaxValue)]
        public int NumCoaches { get; set; } = 10; 
        [Range(typeof(decimal), "0", "1")]
        public decimal BusinessPercentage { get; set; } = 0.20m;
        public bool IsActive { get; set; } = true; 
    }
}