using DEMO_Task_Management_System.Infrastructure.Repositories;
using DEMO_Task_Management_System.Domain.Services;
using DEMO_Task_Management_System.Domain.Entities.Models;
using DEMO_Task_Management_System.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DEMO_Task_Management_System.Application.Dto;
using DEMO_Task_Management_System.Application.Interfaces;

namespace DEMO_Task_Management_System.Application.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEmailService _emailService;
        private readonly ITeamRepository _teamRepository;

        public TasksController(ITasksRepository tasksRepository, IProjectRepository projectRepository, IEmailService emailService, ITeamRepository teamRepository)
        {
            _tasksRepository = tasksRepository;
            _projectRepository = projectRepository;
            _emailService = emailService;
            _teamRepository = teamRepository;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            // Retrieve all tasks from the repository
            var tasks = await _tasksRepository.GetAllTasks();

            // Return the tasks as a response
            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSingleTask([FromRoute] int id)
        {
            // Retrieve a specific task by its ID from the repository
            var task = await _tasksRepository.GetTaskById(id);

            if (task != null)
            {
                // Return the task as a response if found
                return Ok(task);
            }

            // Return 404 Not Found if the task with the given ID is not found
            return NotFound();
        }

        // GET: api/Tasks/Category/CategoryName
        [HttpGet]
        [Route("Category/{category}")]
        public async Task<IActionResult> GetTasksByCategory(string category)
        {
            try
            {
                // Retrieve tasks by their category from the repository
                var tasks = await _tasksRepository.GetTasksByCategory(category);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return 500 Internal Server Error with a message
                return StatusCode(500, "An error occurred while fetching tasks by category.");
            }
        }

        // GET: api/Tasks/Search?category=CategoryName
        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                // Return 400 Bad Request if the category parameter is empty or null
                return BadRequest("Category cannot be empty.");
            }

            // Retrieve tasks by their category from the repository
            var tasks = await _tasksRepository.GetTasksByCategory(category);

            // Return the tasks as a response
            return Ok(tasks);
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<IActionResult> AddTasks(Tasks model)
        {
            // Add the new task to the repository
            await _tasksRepository.AddTask(model);

            // Return the new task in the response body
            return Ok(model);
        }

        // PUT: api/Tasks/5
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto taskUpdateDto)
        {
            // Retrieve the task with the given ID from the repository
            var task = await _tasksRepository.GetTaskById(id);
            if (task == null)
            {
                // Return 404 Not Found if the task with the given ID is not found
                return NotFound($"Task not found with ID: {id}");
            }

            // Update the task properties from the DTO
            task.Title = taskUpdateDto.Title;
            task.Description = taskUpdateDto.Description;
            task.DueDate = taskUpdateDto.DueDate;
            task.Priority = taskUpdateDto.Priority;
            task.IsCompleted = taskUpdateDto.IsCompleted;
            task.Category = taskUpdateDto.Category;
            task.TaskStatus = taskUpdateDto.TaskStatus;
            task.Urgency = taskUpdateDto.Urgency;

            if (taskUpdateDto.ProjectId.HasValue)
            {
                // Retrieve the project with the specified ID from the repository
                var project = await _projectRepository.GetProjectById(taskUpdateDto.ProjectId.Value);
                if (project == null)
                {
                    // Return 404 Not Found if the project with the specified ID is not found
                    return NotFound($"Project not found with ID: {taskUpdateDto.ProjectId.Value}");
                }

                // Assign the task to the specified project
                await _tasksRepository.AssignTaskToProject(id, taskUpdateDto.ProjectId.Value);
            }
            else
            {
                // If the user wants to remove the assignment, set the ProjectId to null
                task.ProjectId = null;
            }

            // Task update notification
            if (task == null)
            {
                // Return 404 Not Found if the task with the given ID is not found (redundant check)
                return NotFound($"Task not found with ID: {id}");
            }

            // Retrieve the authenticated user's ID
            var teamMemberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the team ID associated with the team member making the update
            var teamId = await _teamRepository.GetTeamIdByTeamMemberAsync(teamMemberId);

            // Check if the team member is associated with a team
            if (teamId == null)
            {
                // Handle the case when the team member is not associated with any team
                return BadRequest("You are not a member of any team.");
            }

            // Get all team members' emails and names from the TeamRepository based on the teamId
            var teamMembers = await _teamRepository.GetTeamMembers(teamId.Value);

            // Retrieve the authenticated user's email and name
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            // Send task update notification email to all team members
            foreach (var teamMember in teamMembers)
            {
                await _emailService.SendTaskUpdateNotificationAsync(teamMember.Email, teamMember.UserName, task.Id.ToString(), task.Title, task.TaskStatus.ToString());
            }

            // Update the task in the repository
            await _tasksRepository.UpdateTask(task);

            // Return the updated task in the response body
            return Ok(task);
        }

        // DELETE: api/Tasks/5
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTasks([FromRoute] int id)
        {
            // Retrieve the task with the given ID from the repository
            var task = await _tasksRepository.GetTaskById(id);

            if (task != null)
            {
                // Delete the task from the repository
                await _tasksRepository.DeleteTask(id);

                // Return the deleted task in the response body
                return Ok(task);
            }

            // Return 404 Not Found if the task with the given ID is not found
            return NotFound();
        }

        // GET: api/Tasks/AssignedUsers
        [HttpGet]
        [Route("AssignedUsers")]
        public async Task<IActionResult> GetTasksByAssignedUsers([FromHeader] List<string> usernames)
        {
            try
            {
                // Retrieve tasks by their assigned users from the repository
                var tasks = await _tasksRepository.GetTasksByAssignedUsers(usernames);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return 400 Bad Request with the exception message
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Tasks/1/Assign/
        [HttpPost]
        [Route("{taskId}/Assign/")]
        public async Task<IActionResult> AssignTask(int taskId, [FromHeader] List<string> usernames)
        {
            try
            {
                // Assign the task to the specified users
                await _tasksRepository.AssignTask(taskId, usernames);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // Handle ArgumentException and return 400 Bad Request with the exception message
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Tasks/1/Assign/Update
        [HttpPut]
        [Route("{taskId}/Assign/Update")]
        public async Task<IActionResult> UpdateAssignTask(int taskId, [FromHeader] List<string> usernames)
        {
            try
            {
                // Update the assigned users for the specified task
                await _tasksRepository.UpdateAssignTask(taskId, usernames);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // Handle ArgumentException and return 400 Bad Request with the exception message
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Tasks/1/Assign/2
        [HttpPost]
        [Route("{taskId}/Assign/{projectId}")]
        public async Task<IActionResult> AssignTaskToProject(int taskId, int projectId)
        {
            try
            {
                // Assign the task to the specified project
                await _tasksRepository.AssignTaskToProject(taskId, projectId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                // Handle ArgumentException and return 404 Not Found with the exception message
                return NotFound(ex.Message);
            }
        }

        // GET: api/Tasks/byproject/1
        [HttpGet]
        [Route("byproject/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            // Retrieve the project with the specified ID from the repository
            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                // Return 404 Not Found if the project with the given ID is not found
                return NotFound($"Project not found with ID: {projectId}");
            }

            // Retrieve tasks by their project ID from the repository
            var tasks = await _tasksRepository.GetTasksByProject(projectId);

            // Return the tasks as a response
            return Ok(tasks);
        }

        // POST: api/Tasks/Get-Upcoming-Tasks
        [HttpPost]
        [Route("Get-Upcoming-Tasks")]
        public async Task<IActionResult> GetTasksWithUpcomingDeadlines(int days)
        {
            if (days <= 0)
            {
                // Return 400 Bad Request if the 'days' parameter is less than or equal to zero
                return BadRequest("The 'days' parameter must be greater than zero.");
            }

            // Create a TimeSpan based on the specified days
            var threshold = new TimeSpan(days, 0, 0, 0);

            // Retrieve tasks with upcoming deadlines from the repository based on the threshold
            var upcomingTasks = await _tasksRepository.GetTasksWithUpcomingDeadlines(threshold);

            // Return the upcoming tasks as a response
            return Ok(upcomingTasks);
        }
    }

}
