namespace RailwayReservation.DTOs
{
    public class TrainResponseDTO
    {
        public string TrainNo { get; set; } = null!;
        public string Source { get; set; } = null!;
        public string Destination { get; set; } = null!;
        public string ArrivalTime { get; set; } = null!;
        public string DepartureTime { get; set; } = null!;
        public decimal BaseFare { get; set; }
        public int AvailableSeats { get; set; } 
    }
}