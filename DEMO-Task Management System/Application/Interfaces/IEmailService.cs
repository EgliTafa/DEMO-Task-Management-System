﻿namespace DEMO_Task_Management_System.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendTaskUpdateNotificationAsync(string recipientEmail, string recipientName, string taskId, string taskTitle, string taskStatus);
        Task SendTaskReminderNotificationAsync(string recipientEmail, string recipientName, string taskId, string taskTitle, DateTime dueDate);

    }
}
