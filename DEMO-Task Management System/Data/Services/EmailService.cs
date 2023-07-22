using DEMO_Task_Management_System.Data.Interfaces;
using DEMO_Task_Management_System.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace DEMO_Task_Management_System.Data.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendTaskUpdateNotificationAsync(string recipientEmail, string recipientName, string taskId, string taskTitle, string taskStatus)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var enableSsl = bool.Parse(emailSettings["EnableSsl"]);
            var userName = emailSettings["UserName"];
            var password = emailSettings["Password"];
            var senderEmailAddress = emailSettings["SenderEmailAddress"];
            var senderName = emailSettings["SenderName"];

            recipientName = user.UserName;
            recipientEmail = user.Email;

            if (recipientEmail != null && recipientName != null)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmailAddress));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.Subject = "Task Update Notification";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
                    <h3>Task Update Notification</h3>
                    <p>Your task with ID {taskId} has been updated:</p>
                    <p>Title: {taskTitle}</p>
                    <p>Status: {taskStatus}</p>
                ";

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(userName, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            else
            {
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(recipientEmail));
            }
        }

        public async Task SendTaskReminderNotificationAsync(string recipientEmail, string recipientName, string taskId, string taskTitle, DateTime dueDate)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var enableSsl = bool.Parse(emailSettings["EnableSsl"]);
            var userName = emailSettings["UserName"];
            var password = emailSettings["Password"];
            var senderEmailAddress = emailSettings["SenderEmailAddress"];
            var senderName = emailSettings["SenderName"];

            if (recipientEmail != null && recipientName != null)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmailAddress));
                message.To.Add(new MailboxAddress(recipientName, recipientEmail));
                message.Subject = "Task Reminder Notification";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
                <h3>Task Reminder Notification</h3>
                <p>Your task with ID {taskId} is due soon:</p>
                <p>Title: {taskTitle}</p>
                <p>Due Date: {dueDate.ToString("yyyy-MM-dd HH:mm")}</p>
            ";

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(userName, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            else
            {
                throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(recipientEmail));
            }
        }
    }
}
