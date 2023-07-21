namespace DEMO_Task_Management_System.Data.Interfaces
{
    public interface IEmailService
    {
        Task SendTaskUpdateNotificationAsync(string recipientEmail, string recipientName, string taskId, string taskTitle, string taskStatus);
    }
}
