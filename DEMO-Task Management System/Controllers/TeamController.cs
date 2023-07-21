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
            var teams = await _teamRepository.GetAllAsync();
            return Ok(teams);
        }

        // GET: api/Team/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        // POST: api/Team
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _teamRepository.CreateAsync(team);
            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // PUT: api/Team/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
            {
                return NotFound();
            }

            existingTeam.Name = team.Name;
            existingTeam.Description = team.Description;

            await _teamRepository.UpdateAsync(existingTeam);
            return NoContent();
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            await _teamRepository.DeleteAsync(team);
            return NoContent();
        }

        [HttpPut]
        [Route("AssignUserToTeam/{username}/{teamId}")]
        public async Task<IActionResult> AssignUserToTeam(string username, int teamId)
        {
            var result = await _teamRepository.AssignUserToTeam(username, teamId);
            if (result)
            {
                return Ok("User has been assigned to the team successfully.");
            }
            else
            {
                return BadRequest("Failed to assign user to the team.");
            }
        }
    }
}
