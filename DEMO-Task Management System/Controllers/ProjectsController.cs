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
            // Retrieve all projects from the repository
            var projects = await _projectRepository.GetAllProjects();

            // Return the projects as a response
            return Ok(projects);
        }

        // GET: api/projects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            // Retrieve a specific project by its ID from the repository
            var project = await _projectRepository.GetProjectById(id);
            if (project == null)
            {
                // If the project with the given ID is not found, return 404 Not Found
                return NotFound();
            }

            // Return the project as a response
            return Ok(project);
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> AddProject(Project project)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return 400 Bad Request with error messages
                return BadRequest(ModelState);
            }

            // Add the new project to the repository
            var projectId = await _projectRepository.AddProject(project);

            // Return the new project's ID in the response header and the project in the response body
            return CreatedAtAction(nameof(GetProject), new { id = projectId }, project);
        }

        // PUT: api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            if (id != project.Id)
            {
                // If the provided ID in the URL does not match the project's ID in the request body, return 400 Bad Request
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return 400 Bad Request with error messages
                return BadRequest(ModelState);
            }

            try
            {
                // Update the project in the repository
                await _projectRepository.UpdateProject(project);
            }
            catch (ArgumentException)
            {
                // If the project with the given ID is not found in the repository, return 404 Not Found
                return NotFound();
            }

            // Return 204 No Content to indicate successful update without any response body
            return NoContent();
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                // Delete the project with the given ID from the repository
                await _projectRepository.DeleteProject(id);
            }
            catch (ArgumentException)
            {
                // If the project with the given ID is not found in the repository, return 404 Not Found
                return NotFound();
            }

            // Return 204 No Content to indicate successful deletion without any response body
            return NoContent();
        }
    }
}
