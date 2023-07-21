using DEMO_Task_Management_System.Data;
using DEMO_Task_Management_System.Data.Interfaces;
using DEMO_Task_Management_System.Data.Repositories;
using DEMO_Task_Management_System.Dto;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DEMO_Task_Management_System.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _tasksRepository.GetAllTasks();
            return Ok(tasks);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSingleTask([FromRoute] int id)
        {
            var task = await _tasksRepository.GetTaskById(id);

            if (task != null)
            {
                return Ok(task);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("Category/{category}")]
        public async Task<IActionResult> GetTasksByCategory(string category)
        {
            try
            {
                var tasks = await _tasksRepository.GetTasksByCategory(category);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching tasks by category.");
            }
        }

        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> SearchTasks([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Category cannot be empty.");
            }

            var tasks = await _tasksRepository.GetTasksByCategory(category);
            return Ok(tasks);
        }


        [HttpPost]
        public async Task<IActionResult> AddTasks(Tasks model)
        {
            await _tasksRepository.AddTask(model);
            return Ok(model);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskUpdateDto taskUpdateDto)
        {
            var task = await _tasksRepository.GetTaskById(id);
            if (task == null)
            {
                return NotFound($"Task not found with ID: {id}");
            }

            // Update the task properties from the DTO
            task.Title = taskUpdateDto.Title;
            task.Description = taskUpdateDto.Description;
            task.DueDate = taskUpdateDto.DueDate;
            task.Priority = taskUpdateDto.Priority;
            task.IsCompleted = taskUpdateDto.IsCompleted;
            task.Category = taskUpdateDto.Category;

            // Update the task status based on the provided TaskStatusEnum value
            task.TaskStatus = taskUpdateDto.TaskStatus;

            if (taskUpdateDto.ProjectId.HasValue)
            {
                var project = await _projectRepository.GetProjectById(taskUpdateDto.ProjectId.Value);
                if (project == null)
                {
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

            await _tasksRepository.UpdateTask(task);

            return Ok(task);
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTasks([FromRoute] int id)
        {
            var task = await _tasksRepository.GetTaskById(id);

            if (task != null)
            {
                await _tasksRepository.DeleteTask(id);
                return Ok(task);
            }

            return NotFound();
        }
        //Get tasks by users
        [HttpGet]
        [Route("AssignedUsers")]
        public async Task<IActionResult> GetTasksByAssignedUsers([FromHeader] List<string> usernames)
        {
            try
            {
                var tasks = await _tasksRepository.GetTasksByAssignedUsers(usernames);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Assign tasks to users
        [HttpPost]
        [Route("{taskId}/Assign/")]
        public async Task<IActionResult> AssignTask(int taskId, [FromHeader] List<string> usernames)
        {
            try
            {
                await _tasksRepository.AssignTask(taskId, usernames);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Update the users on a task
        [HttpPut]
        [Route("{taskId}/Assign/Update")]
        public async Task<IActionResult> UpdateAssignTask(int taskId, [FromHeader] List<string> usernames)
        {
            try
            {
                await _tasksRepository.UpdateAssignTask(taskId, usernames);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Assign tasks to projects
        [HttpPost]
        [Route("{taskId}/Assign/{projectId}")]
        public async Task<IActionResult> AssignTaskToProject(int taskId, int projectId)
        {
            try
            {
                await _tasksRepository.AssignTaskToProject(taskId, projectId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("byproject/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var project = await _projectRepository.GetProjectById(projectId);
            if (project == null)
            {
                return NotFound($"Project not found with ID: {projectId}");
            }

            var tasks = await _tasksRepository.GetTasksByProject(projectId);
            return Ok(tasks);
        }
    }
}
