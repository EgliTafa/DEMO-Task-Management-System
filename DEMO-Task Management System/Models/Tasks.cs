using DEMO_Task_Management_System.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json.Serialization;

namespace DEMO_Task_Management_System.Models
{
    public class Tasks
    {
        [JsonIgnore] // Exclude the Id property from serialization
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskStatusList TaskStatus { get; set; } = TaskStatusList.NotStarted;
        public bool IsCompleted { get; set; } = false;
        [JsonIgnore] 
        public string? UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        // Foreign key for the associated project
        [JsonIgnore]
        public int? ProjectId { get; set; }
        [JsonIgnore]
        public Project? Project { get; set; }

    }
}
