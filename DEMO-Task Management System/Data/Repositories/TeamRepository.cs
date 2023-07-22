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

        // Constructor for the TeamRepository class that takes an instance of the ApplicationDbContext
        // as a parameter. This dependency is injected via dependency injection.
        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get a team by its ID from the database.
        // Parameters:
        //   - id: The ID of the team to retrieve.
        // Returns: The team as a Team object if found, otherwise null.
        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        // Get the members of a team by the team's ID.
        // Parameters:
        //   - teamId: The ID of the team for which members are to be retrieved.
        // Returns: A list of User objects representing the members of the specified team.
        public async Task<List<User>> GetTeamMembers(int teamId)
        {
            var team = await _context.Teams.Include(t => t.TeamMembers).FirstOrDefaultAsync(t => t.Id == teamId);
            return team?.TeamMembers ?? new List<User>();
        }

        // Get the ID of the team to which a team member belongs.
        // Parameters:
        //   - teamMemberId: The ID of the team member (User) for which the team ID is to be retrieved.
        // Returns: The ID of the team to which the team member belongs, or null if not found.
        public async Task<int?> GetTeamIdByTeamMemberAsync(string teamMemberId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamMembers.Any(u => u.Id == teamMemberId));
            return team?.Id;
        }

        // Get all teams from the database.
        // Returns: A list of all teams as IEnumerable<Team>.
        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        // Create a new team and add it to the database.
        // Parameters:
        //   - team: The Team object representing the new team to be created.
        public async Task CreateAsync(Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
        }

        // Update an existing team in the database.
        // Parameters:
        //   - team: The Team object with updated values.
        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        // Delete a team from the database.
        // Parameters:
        //   - team: The Team object representing the team to be deleted.
        public async Task DeleteAsync(Team team)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        // Find a user by their username in the database.
        // Parameters:
        //   - userName: The username of the user to retrieve.
        // Returns: The User object representing the user with the specified username, or null if not found.
        public async Task<User> FindUserByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        // Assign a user (team member) to a team.
        // Parameters:
        //   - userName: The username of the user to be assigned to the team.
        //   - teamId: The ID of the team to which the user will be assigned.
        // Returns: True if the user was successfully assigned to the team, otherwise false.
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
