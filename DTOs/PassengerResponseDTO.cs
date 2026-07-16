using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailConnect.DTOs
{
    public class PassengerResponseDTO
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string? CoachNo { get; set; }
        public string? SeatNo { get; set; }
    }
}