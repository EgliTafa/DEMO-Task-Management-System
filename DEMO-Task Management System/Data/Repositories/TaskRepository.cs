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
            return await _context.Tasks.ToListAsync();
        }

        public async Task<Tasks> GetTaskById(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<Tasks>> GetTasksByCategory(string category)
        {
            var tasks = await _context.Tasks
                .Where(t => t.Category == category)
                .ToListAsync();

            return tasks;
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


        public async Task<IEnumerable<TaskAssignment>> GetTasksByAssignedUsers(List<string> usernames)
        {
            // Get the tasks assigned to the specified users
            var tasks = await _context.TaskAssignments
                .Where(x => usernames.Contains(x.User.UserName))
                .ToListAsync();

            return tasks;
        }

        public async Task<IEnumerable<Tasks>> SearchTasksByCategory(string category, string searchQuery)
        {
            var tasks = await _context.Tasks
                .Where(task => task.Category == category && task.Category.Contains(searchQuery))   
                .ToListAsync();

            return tasks;
        }

        //Assigns tasks to projects
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

        public async Task<IEnumerable<Tasks>> GetTasksByProject(int projectId)
        {
            var tasksWithProject = await _context.Tasks.Where(t => t.ProjectId == projectId).ToListAsync();

            return tasksWithProject;
        }
    }
}
