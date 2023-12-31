﻿using DEMO_Task_Management_System.Domain.Entities.Models;

namespace DEMO_Task_Management_System.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<Project> GetProjectById(int id);
        Task<int> AddProject(Project project);
        Task UpdateProject(Project project);
        Task DeleteProject(int id);
    }

}
