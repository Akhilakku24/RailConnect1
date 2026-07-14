using System.ComponentModel.DataAnnotations;

namespace RailwayReservation.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string? CoachNo { get; set; }
        public string? SeatNo { get; set; }
        
        public int BookingId { get; set; }
    }
}