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
        public int? ProjectId { get; set; }
    }
}
