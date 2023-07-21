using DEMO_Task_Management_System.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DEMO_Task_Management_System.Dto
{
    public class TaskUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int Priority { get; set; }
        public string? Category { get; set; }
        public bool IsCompleted { get; set; }
        [JsonIgnore]
        public int? ProjectId { get; set; }
        public TaskStatusList TaskStatus { get; set; }
        [NotMapped] // This attribute tells Entity Framework not to map this property to the database.
        public string TaskStatusText => Enum.GetName(typeof(TaskStatusList), TaskStatus);
    }
}
