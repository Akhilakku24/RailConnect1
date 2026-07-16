using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RailwayReservation.DTOs;

namespace RailConnect.DTOs
{
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