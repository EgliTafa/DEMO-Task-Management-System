using System.ComponentModel.DataAnnotations;

namespace DEMO_Task_Management_System.Dto
{
    public class UserRegistrationDto
    {
        // The username provided by the user during registration.

        [Required]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(60)]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
