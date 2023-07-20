using DEMO_Task_Management_System.Data.Interfaces;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DEMO_Task_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectRepository.GetAllProjects();
            return Ok(projects);
        }

        // GET: api/projects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _projectRepository.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> AddProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var projectId = await _projectRepository.AddProject(project);
            return CreatedAtAction(nameof(GetProject), new { id = projectId }, project);
        }

        // PUT: api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _projectRepository.UpdateProject(project);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectRepository.DeleteProject(id);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}
