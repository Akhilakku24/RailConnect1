using Microsoft.AspNetCore.Identity;

namespace RailwayReservation.Models
{
    
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        
    }
}