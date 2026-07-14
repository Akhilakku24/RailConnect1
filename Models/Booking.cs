using System.ComponentModel.DataAnnotations;

namespace RailwayReservation.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        public string PNR { get; set; } = null!;
        public string UserId { get; set; } = null!; 
        public int TrainId { get; set; } 
        public DateTime JourneyDate { get; set; }
        public string ClassType { get; set; } = "Economy";
        public string Quota { get; set; } = "General";
        public decimal TotalFare { get; set; }
        
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = "Confirmed"; 
        public List<Passenger> Passengers { get; set; } = new();
    }
}