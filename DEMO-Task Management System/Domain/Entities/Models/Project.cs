using System.Text.Json.Serialization;

namespace DEMO_Task_Management_System.Domain.Entities.Models
{
    public class Project
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } // Timestamp to track project creation time
        public string Description { get; set; } // Optional description for the project
    }

}
