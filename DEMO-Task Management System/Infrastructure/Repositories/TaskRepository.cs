using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DEMO_Task_Management_System.Domain.Services;
using DEMO_Task_Management_System.Domain.Entities.Models;
using DEMO_Task_Management_System.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DEMO_Task_Management_System.Application.Interfaces;
using DEMO_Task_Management_System.Infrastructure.Data;

namespace DEMO_Task_Management_System.Infrastructure.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserManager<User> _userManager;
        private IEmailService _emailService;

        // Constructor for the TasksRepository class that takes an instance of the ApplicationDbContext,
        // IHttpContextAccessor, UserManager<User>, and IEmailService as parameters. These dependencies
        // are injected via dependency injection.
        public TasksRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IEmailService emailService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _emailService = emailService;
        }

        // Get all tasks from the database.
        // Returns: A list of tasks as IEnumerable<Tasks>.
        public async Task<IEnumerable<Tasks>> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return tasks;
        }

        // Get a specific task by its ID from the database.
        // Parameters:
        //   - id: The ID of the task to retrieve.
        // Returns: The task as a Tasks object if found, otherwise null.
        public async Task<Tasks> GetTaskById(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            return task;
        }

        // Get all tasks with a specific category from the database.
        // Parameters:
        //   - category: The category of tasks to retrieve.
        // Returns: A list of tasks with the specified category as IEnumerable<Tasks>.
        public async Task<IEnumerable<Tasks>> GetTasksByCategory(string category)
        {
            var tasks = await _context.Tasks
                .Where(t => t.Category == category)
                .ToListAsync();

            return tasks;
        }

        // Add a new task to the database.
        // Parameters:
        //   - task: The task object to be added to the database.
        public async Task AddTask(Tasks task)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            //Tracks the user who created the task
            task.UserId = user.Id;
            task.User = user;
            task.IsCompleted = false;
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        // Update an existing task in the database.
        // Parameters:
        //   - task: The task object with updated values.
        public async Task UpdateTask(Tasks task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        // Delete a task from the database based on its ID.
        // Parameters:
        //   - id: The ID of the task to be deleted.
        public async Task DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        // Assign a task to a list of team members.
        // Parameters:
        //   - taskId: The ID of the task to be assigned.
        //   - usernames: The list of usernames (team members) to whom the task will be assigned.
        public async Task AssignTask(int taskId, List<string> usernames)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException($"Task not found with ID: {taskId}");
            }

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Check if the user is associated with a team
            if (user.TeamId == null)
            {
                throw new InvalidOperationException("User must be associated with a team to assign tasks.");
            }

            // Get the team members associated with the user's team
            var teamMembers = await _context.Users
                .Where(u => u.TeamId == user.TeamId && usernames.Contains(u.UserName))
                .ToListAsync();

            if (teamMembers.Count != usernames.Count)
            {
                // Some of the specified usernames are not team members
                throw new ArgumentException("Some of the specified usernames are not team members.");
            }

            // Clear existing task assignments for the task
            var existingAssignments = _context.TaskAssignments.Where(ta => ta.TaskId == taskId).ToList();
            _context.TaskAssignments.RemoveRange(existingAssignments);

            foreach (var teamMember in teamMembers)
            {
                var taskAssignment = new TaskAssignment
                {
                    Task = task,
                    User = teamMember
                };

                _context.TaskAssignments.Add(taskAssignment);
            }

            await _context.SaveChangesAsync();
        }

        // Update the task assignments for a task based on a list of team members.
        // Parameters:
        //   - taskId: The ID of the task whose assignments are being updated.
        //   - usernames: The updated list of usernames (team members) to whom the task will be assigned.
        public async Task UpdateAssignTask(int taskId, List<string> usernames)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException($"Task not found with ID: {taskId}");
            }

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Check if the user is associated with a team
            if (user.TeamId == null)
            {
                throw new InvalidOperationException("User must be associated with a team to assign tasks.");
            }

            // Get the team members associated with the user's team
            var teamMembers = await _context.Users
                .Where(u => u.TeamId == user.TeamId && usernames.Contains(u.UserName))
                .ToListAsync();

            if (teamMembers.Count != usernames.Count)
            {
                // Some of the specified usernames are not team members
                throw new ArgumentException("Some of the specified usernames are not team members.");
            }

            // Get the existing task assignments for the task
            var existingAssignments = _context.TaskAssignments.Where(ta => ta.TaskId == taskId).ToList();

            foreach (var teamMember in teamMembers)
            {
                // Check if the user is already assigned to the task
                var existingAssignment = existingAssignments.FirstOrDefault(ta => ta.UserId == teamMember.Id);

                if (existingAssignment == null)
                {
                    // If the user is not already assigned, create a new assignment
                    var taskAssignment = new TaskAssignment
                    {
                        Task = task,
                        User = teamMember
                    };

                    _context.TaskAssignments.Add(taskAssignment);
                }
                else
                {
                    // If the user is already assigned, update the existing assignment
                    existingAssignment.User = teamMember;
                    _context.TaskAssignments.Update(existingAssignment);
                }
            }

            // Remove task assignments for team members who are no longer specified
            var removedAssignments = existingAssignments.Where(ta => !teamMembers.Any(tm => tm.Id == ta.UserId));
            _context.TaskAssignments.RemoveRange(removedAssignments);

            await _context.SaveChangesAsync();
        }

        // Get tasks assigned to specific users (team members).
        // Parameters:
        //   - usernames: The list of usernames (team members) for whom the tasks are to be retrieved.
        // Returns: A list of tasks assigned to the specified users as IEnumerable<TaskAssignment>.
        public async Task<IEnumerable<TaskAssignment>> GetTasksByAssignedUsers(List<string> usernames)
        {
            // Get the tasks assigned to the specified users
            var tasks = await _context.TaskAssignments
                .Where(x => usernames.Contains(x.User.UserName))
                .ToListAsync();

            return tasks;
        }

        // Search for tasks with a specific category and matching search query.
        // Parameters:
        //   - category: The category of tasks to search within.
        //   - searchQuery: The search query string.
        // Returns: A list of tasks with the specified category and matching search query as IEnumerable<Tasks>.
        public async Task<IEnumerable<Tasks>> SearchTasksByCategory(string category, string searchQuery)
        {
            var tasks = await _context.Tasks
                .Where(task => task.Category == category && task.Category.Contains(searchQuery))
                .ToListAsync();

            return tasks;
        }

        // Assign a task to a project.
        // Parameters:
        //   - taskId: The ID of the task to be assigned to the project.
        //   - projectId: The ID of the project to which the task will be assigned.
        public async Task AssignTaskToProject(int taskId, int projectId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException($"Task not found with ID: {taskId}");
            }

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null)
            {
                throw new ArgumentException($"Project not found with ID: {projectId}");
            }

            task.ProjectId = projectId;
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        // Get tasks associated with a specific project.
        // Parameters:
        //   - projectId: The ID of the project for which tasks are to be retrieved.
        // Returns: A list of tasks associated with the specified project as IEnumerable<Tasks>.
        public async Task<IEnumerable<Tasks>> GetTasksByProject(int projectId)
        {
            var tasksWithProject = await _context.Tasks.Where(t => t.ProjectId == projectId).ToListAsync();

            return tasksWithProject;
        }

        // Get tasks with upcoming deadlines within a given threshold.
        // Parameters:
        //   - threshold: The time span representing the threshold for upcoming tasks.
        // Returns: A list of tasks with deadlines within the specified threshold as IEnumerable<Tasks>.
        public async Task<IEnumerable<Tasks>> GetTasksWithUpcomingDeadlines(TimeSpan threshold)
        {
            var currentDate = DateTime.Now;
            var upcomingDate = currentDate.AddDays(threshold.Days);

            var upcomingTasks = await _context.Tasks
                .Include(task => task.User) // Eagerly load the User navigation property
                .Where(task => task.DueDate > currentDate && task.DueDate <= upcomingDate)
                .ToListAsync();

            // Send reminder notifications for each upcoming task
            foreach (var task in upcomingTasks)
            {
                var assignedUser = task.User;
                if (assignedUser != null)
                {
                    await _emailService.SendTaskReminderNotificationAsync(assignedUser.Email, assignedUser.UserName, task.Id.ToString(), task.Title, task.DueDate);
                }
            }

            return upcomingTasks;
        }
    }
}
