using System.ComponentModel.DataAnnotations;

namespace DEMO_Task_Management_System.Dto
{
    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
