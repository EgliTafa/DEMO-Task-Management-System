using DEMO_Task_Management_System.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
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
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DueDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskStatusList TaskStatus { get; set; } = TaskStatusList.NotStarted;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskPriority Priority { get; set; } = TaskPriority.Low;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskUrgency Urgency { get; set; } = TaskUrgency.Low;
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
