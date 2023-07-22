using System.Collections.Generic;
using System.Threading.Tasks;
using DEMO_Task_Management_System.Domain.Entities.Models;

namespace DEMO_Task_Management_System.Domain.Interfaces
{
    public interface ITeamRepository
    {
        Task<Team> GetByIdAsync(int id);
        Task<IEnumerable<Team>> GetAllAsync();
        Task<List<User>> GetTeamMembers(int teamId);
        Task<int?> GetTeamIdByTeamMemberAsync(string teamMemberId);
        Task CreateAsync(Team team);
        Task UpdateAsync(Team team);
        Task DeleteAsync(Team team);
        Task<User> FindUserByNameAsync(string userName);
        Task<bool> AssignUserToTeam(string userId, int teamId);

    }
}
