using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DEMO_Task_Management_System.Models
{
    public class User : IdentityUser
    {
        [MaxLength(20)]
        public string? FirstName { get; set; }
        [MaxLength(60)]
        public string? LastName { get; set; }

        public string? Role { get; set; }
    }
}
