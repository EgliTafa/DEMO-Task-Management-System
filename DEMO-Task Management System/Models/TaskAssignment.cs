namespace DEMO_Task_Management_System.Models
{
    public class TaskAssignment
    {
        public int Id { get; set; }
        public int TaskId { get; set; } // Foreign key for Tasks
        public Tasks Task { get; set; } // Navigation property for the task
        public string UserId { get; set; } // Foreign key for Users
        public User User { get; set; } // Navigation property for the user
    }
}