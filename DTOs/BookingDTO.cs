namespace RailwayReservation.DTOs
{
    public class BookingRequestDTO
    {
        public string TrainNo { get; set; } = null!;
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string ClassType { get; set; } = null!; // Economy, Business, or AC
        public string Quota { get; set; } = "General";
        public string ContactAddress { get; set; } = null!;
        public string CreditCardNo { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public DateTime JourneyDate { get; set; }
        public List<PassengerRequestDTO> Passengers { get; set; } = new();
    }
    public class PassengerRequestDTO
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
    }

    public class PassengerResponseDTO
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string? CoachNo { get; set; }
        public string? SeatNo { get; set; }
    }
    public class BookingResponseDTO
    {
        public string PNR { get; set; } = null!;
        public string TrainNo { get; set; } = null!;
        public decimal TotalFare { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = null!;
        public List<PassengerResponseDTO> Passengers { get; set; } = new();
    }
}