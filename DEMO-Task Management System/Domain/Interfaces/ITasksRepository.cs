using System.Collections.Generic;
using System.Threading.Tasks;
using DEMO_Task_Management_System.Domain.Entities.Models;

namespace DEMO_Task_Management_System.Domain.Interfaces
{
    public interface ITasksRepository
    {
        Task<IEnumerable<Tasks>> GetAllTasks();
        Task<Tasks> GetTaskById(int id);
        Task AddTask(Tasks task);
        Task UpdateTask(Tasks task);
        Task DeleteTask(int id);
        Task AssignTask(int taskId, List<string> usernames);
        Task AssignTaskToProject(int taskId, int projectId);
        Task<IEnumerable<Tasks>> GetTasksByCategory(string category);
        Task<IEnumerable<Tasks>> SearchTasksByCategory(string category, string searchQuery);
        Task<IEnumerable<Tasks>> GetTasksByProject(int projectId);
        Task UpdateAssignTask(int taskId, List<string> usernames);
        Task<IEnumerable<TaskAssignment>> GetTasksByAssignedUsers(List<string> usernames);
        Task<IEnumerable<Tasks>> GetTasksWithUpcomingDeadlines(TimeSpan threshold);

    }
}
