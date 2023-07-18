using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DEMO_Task_Management_System.Data
{
    public class TasksRepository : ITasksRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<User> _userManager;

        public TasksRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Tasks>> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            PrintTaskStatusMessages(tasks);
            return await _context.Tasks.ToListAsync();
        }

        public async Task<Tasks> GetTaskById(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            PrintTaskStatusMessage(task);
            return await _context.Tasks.FindAsync(id);
        }

        public async Task AddTask(Tasks task)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            //Tracks the user who created the task
            task.UserId = user.Id;
            task.User = user;
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateTask(Tasks task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task AssignTask(int taskId, List<string> usernames)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException($"Task not found with ID: {taskId}");
            }

            foreach (var username in usernames)
            {
                var assignedUser = await _userManager.FindByNameAsync(username);
                if (assignedUser == null)
                {
                    throw new ArgumentException($"Invalid username specified: {username}");
                }

                var taskAssignment = new TaskAssignment
                {
                    Task = task,
                    User = assignedUser
                };

                _context.TaskAssignments.Add(taskAssignment);
            }

            await _context.SaveChangesAsync();
        }

        private void PrintTaskStatusMessage(Tasks task)
        {
            if (task != null)
            {
                if (task.IsCompleted)
                {
                    Console.WriteLine("Task completed");
                }
                else
                {
                    Console.WriteLine("Task underway");
                }
            }
        }

        private void PrintTaskStatusMessages(IEnumerable<Tasks> tasks)
        {
            foreach (var task in tasks)
            {
                PrintTaskStatusMessage(task);
            }
        }

    }
}
