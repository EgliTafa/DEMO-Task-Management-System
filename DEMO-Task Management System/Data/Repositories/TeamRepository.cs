using DEMO_Task_Management_System.Data.Interfaces;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEMO_Task_Management_System.Data.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task<List<User>> GetTeamMembers(int teamId)
        {
            var team = await _context.Teams.Include(t => t.TeamMembers).FirstOrDefaultAsync(t => t.Id == teamId);
            return team?.TeamMembers ?? new List<User>();
        }

        public async Task<int?> GetTeamIdByTeamMemberAsync(string teamMemberId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamMembers.Any(u => u.Id == teamMemberId));
            return team?.Id;
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task CreateAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Team team)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindUserByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> AssignUserToTeam(string userName, int teamId)
        {
            var user = await FindUserByNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
            {
                return false;
            }

            user.TeamId = teamId;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Handle any exceptions, log errors, and return false on failure
                return false;
            }
        }
    }

}
