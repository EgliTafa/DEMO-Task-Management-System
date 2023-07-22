﻿using DEMO_Task_Management_System.Dto;
using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DEMO_Task_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        // Endpoint for user registration
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto model)
        {
            // Validate the incoming request data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new User object with the provided registration data
            var user = new User
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role,
                Email = model.Email
            };

            // Attempt to create the user in the database
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                // Return bad request with error messages if user creation fails
                return BadRequest(result.Errors);
            }

            // Check if the provided role exists, and create it if it doesn't
            if (!string.IsNullOrEmpty(model.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.Role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                }

                // Add the user to the specified role
                var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!roleResult.Succeeded)
                {
                    // Return bad request with error messages if role assignment fails
                    return BadRequest(roleResult.Errors);
                }
            }

            // Return success if user registration is successful
            return Ok();
        }

        // Endpoint for user login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            // Validate the incoming request data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find the user by username
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Return unauthorized if the user is not found
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Check if the provided password is correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                // Return unauthorized if the password is incorrect
                return Unauthorized(new { message = "Invalid username or password." });
            }

            // Generate a JWT token for the authenticated user
            var token = GenerateJwtToken(user);

            // Set the authentication token as a response header
            Response.Headers.Add("Authorization", "Bearer " + token);

            // Return success with the JWT token
            return Ok(new { Token = token });
        }

        // Generate a JWT token for the authenticated user
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings.GetValue<string>("Key"));
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        // Endpoint for user logout
        [HttpPost]
        [Route("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Sign out the current user
            await _signInManager.SignOutAsync();

            // Return success
            return Ok();
        }
    }
}

