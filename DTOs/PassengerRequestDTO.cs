using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RailConnect.DTOs
{
    public class PassengerRequestDTO
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
    }
}