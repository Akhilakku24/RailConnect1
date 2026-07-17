using System.ComponentModel.DataAnnotations;
using RailConnect.DTOs;

namespace RailwayConnect.DTOs
{
    public class BookingRequestDTO
    {
        [Required]
        public string TrainNo { get; set; } = null!;
        [Range(0, 8)]
        public int AdultCount { get; set; }
        [Range(0, 8)]
        public int ChildCount { get; set; }
        [Required]
        public string ClassType { get; set; } = null!; // Economy, Business
        public string Quota { get; set; } = "General";
        [Required]
        public string ContactAddress { get; set; } = null!;
        [Required]
        public string CreditCardNo { get; set; } = null!;
        [Required]
        public string BankName { get; set; } = null!;
        [Required]
        public DateTime JourneyDate { get; set; }
        [MinLength(1)]
        public List<PassengerRequestDTO> Passengers { get; set; } = new();
    }
    
}