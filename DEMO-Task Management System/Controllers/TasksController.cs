using DEMO_Task_Management_System.Data;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DEMO_Task_Management_System.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITasksRepository _tasksRepository;

        public TasksController(ITasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
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

        [HttpPost]
        public async Task<IActionResult> AddTasks(Tasks model)
        {
            await _tasksRepository.AddTask(model);
            return Ok(model);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTask(Tasks model, [FromRoute] int id)
        {
            var task = await _tasksRepository.GetTaskById(id);

            if (task != null)
            {
                task.Title = model.Title;
                task.Description = model.Description;
                task.DueDate = model.DueDate;
                task.IsCompleted = model.IsCompleted;
                task.Priority = model.Priority;

                await _tasksRepository.UpdateTask(task);

                return Ok(task);
            }

            return NotFound();
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
        [HttpPost]
        [Route("{taskId}/assign/")]
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

    }
}
