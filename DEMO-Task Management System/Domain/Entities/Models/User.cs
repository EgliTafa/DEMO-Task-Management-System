using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DEMO_Task_Management_System.Domain.Entities.Models
{
    public class User : IdentityUser
    {
        [MaxLength(20)]
        public string? FirstName { get; set; }
        [MaxLength(60)]
        public string? LastName { get; set; }
        public string? Role { get; set; }
        // Foreign key for the associated teams
        public int? TeamId { get; set; } // Nullable TeamId to represent that a user may not belong to a team
        public Team Team { get; set; } // Navigation property to access the associated Team entity
    }
}
