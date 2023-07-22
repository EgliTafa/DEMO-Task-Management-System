using DEMO_Task_Management_System.Data.Interfaces;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DEMO_Task_Management_System.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;

        public TeamController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        // GET: api/Team
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            // Retrieve all teams from the repository
            var teams = await _teamRepository.GetAllAsync();

            // Return the teams as a response
            return Ok(teams);
        }

        // GET: api/Team/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            // Retrieve a specific team by its ID from the repository
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
            {
                // Return 404 Not Found if the team with the given ID is not found
                return NotFound();
            }
            // Return the team as a response
            return Ok(team);
        }

        // POST: api/Team
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            if (!ModelState.IsValid)
            {
                // Return 400 Bad Request if the request body does not contain valid data
                return BadRequest(ModelState);
            }
            // Create a new team in the repository
            await _teamRepository.CreateAsync(team);

            // Return 201 Created with the location of the newly created team in the response header
            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // PUT: api/Team/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] Team team)
        {
            if (id != team.Id)
            {
                // Return 400 Bad Request if the ID in the request path does not match the team ID in the request body
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                // Return 400 Bad Request if the request body does not contain valid data
                return BadRequest(ModelState);
            }

            // Retrieve the existing team with the given ID from the repository
            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
            {
                // Return 404 Not Found if the team with the given ID is not found
                return NotFound();
            }

            // Update the existing team's properties with the new data from the request body
            existingTeam.Name = team.Name;
            existingTeam.Description = team.Description;

            // Update the team in the repository
            await _teamRepository.UpdateAsync(existingTeam);

            // Return 204 No Content since the team has been successfully updated
            return NoContent();
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            // Retrieve the team with the given ID from the repository
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
            {
                // Return 404 Not Found if the team with the given ID is not found
                return NotFound();
            }

            // Delete the team from the repository
            await _teamRepository.DeleteAsync(team);

            // Return 204 No Content since the team has been successfully deleted
            return NoContent();
        }

        // PUT: api/Team/AssignUserToTeam/{username}/{teamId}
        [HttpPut]
        [Route("AssignUserToTeam/{username}/{teamId}")]
        public async Task<IActionResult> AssignUserToTeam(string username, int teamId)
        {
            // Attempt to assign the user with the specified username to the team with the specified teamId
            var result = await _teamRepository.AssignUserToTeam(username, teamId);
            if (result)
            {
                // Return 200 OK with a success message if the user has been assigned to the team successfully
                return Ok("User has been assigned to the team successfully.");
            }
            else
            {
                // Return 400 Bad Request with an error message if the assignment failed
                return BadRequest("Failed to assign user to the team.");
            }
        }
    }
}
