using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RailConnect.DTOs
{
    public class PassengerRequestDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        [Range(0,120)]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; } = null!;
    }
}