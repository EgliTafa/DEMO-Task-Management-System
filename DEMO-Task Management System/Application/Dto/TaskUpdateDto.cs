using DEMO_Task_Management_System.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DEMO_Task_Management_System.Application.Dto
{
    public class TaskUpdateDto
    {
        // The properties represent the updated information for a task.

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Category { get; set; }
        public bool IsCompleted { get; set; }
        [JsonIgnore]
        public int? ProjectId { get; set; }
        public TaskStatusList TaskStatus { get; set; }
        public TaskUrgency Urgency { get; set; }
        public TaskPriority Priority { get; set; }
        [NotMapped] // This attribute tells Entity Framework not to map this property to the database.
        public string TaskStatusText => Enum.GetName(typeof(TaskStatusList), TaskStatus);
        [NotMapped] // This attribute tells Entity Framework not to map this property to the database.
        public string TaskPriorityText => Enum.GetName(typeof(TaskPriority), Priority);
        [NotMapped] // This attribute tells Entity Framework not to map this property to the database.
        public string TaskUrgencyText => Enum.GetName(typeof(TaskUrgency), Urgency);
    }
}
