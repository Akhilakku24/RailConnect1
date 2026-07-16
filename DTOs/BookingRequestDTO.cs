using RailConnect.DTOs;

namespace RailwayConnect.DTOs
{
    public class BookingRequestDTO
    {
        public string TrainNo { get; set; } = null!;
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string ClassType { get; set; } = null!; // Economy, Business
        public string Quota { get; set; } = "General";
        public string ContactAddress { get; set; } = null!;
        public string CreditCardNo { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public DateTime JourneyDate { get; set; }
        public List<PassengerRequestDTO> Passengers { get; set; } = new();
    }
    
}