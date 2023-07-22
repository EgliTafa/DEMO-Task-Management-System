using DEMO_Task_Management_System.Data.Interfaces;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEMO_Task_Management_System.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor for the ProjectRepository class that takes an instance of the ApplicationDbContext
        // as a parameter. The ApplicationDbContext is injected via dependency injection.
        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all projects from the database.
        // Returns: A list of projects as an IEnumerable<Project>.
        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        // Get a specific project by its ID from the database.
        // Parameters:
        //   - id: The ID of the project to retrieve.
        // Returns: The project as a Project object if found, otherwise null.
        public async Task<Project> GetProjectById(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        // Add a new project to the database.
        // Parameters:
        //   - project: The project object to be added to the database.
        // Returns: The ID of the newly added project as an integer.
        public async Task<int> AddProject(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project.Id;
        }

        // Update an existing project in the database.
        // Parameters:
        //   - project: The project object with updated values.
        public async Task UpdateProject(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }

        // Delete a project from the database based on its ID.
        // Parameters:
        //   - id: The ID of the project to be deleted.
        public async Task DeleteProject(int id)
        {
            // Find the project by its ID
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                // Remove the project from the Projects DbSet and save the changes
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }
    }
}
