using System.ComponentModel.DataAnnotations;

namespace DEMO_Task_Management_System.Dto
{
    public class UserLoginDto
    {
        // The username provided by the user during login.

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
