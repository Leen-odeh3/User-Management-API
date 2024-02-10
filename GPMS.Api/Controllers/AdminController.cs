using GPMS.Core.Interfaces;
using GPMS.Core.Models.Authentication.SignUp;
using GPMS.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AdminController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {

            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exists!" });
            }
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username,
                TwoFactorEnabled = true
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User Failed to Create" });
                }
                await _userManager.AddToRoleAsync(user, role);

                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User created SuccessFully" });
            
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This Role Doesnot Exist." });
            }

        }

        [HttpGet("GetAllUsers")]  
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
             
                var allUsers = await _userManager.Users.ToListAsync(); 

                return Ok(allUsers);
            }
            catch (Exception ex)
            {
                // Return an error response
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "An error occurred while fetching users." });
            }
        }

       

        [HttpDelete("DeleteUser/{userId}")]  // Define your route for the DELETE request, for example, "api/auth/DeleteUser/{userId}"
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                
                var user = await _userManager.FindByIdAsync(userId);

              
                if (user == null)
                {
                    return NotFound(new Response { Status = "Error", Message = "User not found." });
                }

              
                var result = await _userManager.DeleteAsync(user);

              
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success", Message = "User deleted successfully." });
                }
                else
                {
   
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Failed to delete user." });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "An error occurred while deleting the user." });
            }
        }

        [HttpPut("UpdateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] RegisterUser updateUserDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId) || updateUserDto == null)
                {
                    return BadRequest(new Response { Status = "Error", Message = "Invalid input parameters." });
                }

                // Find the user by userId
                var user = await _userManager.FindByIdAsync(userId);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound(new Response { Status = "Error", Message = "User not found." });
                }

                // Update user information
                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Username;

                // Update the user in the database
                var result = await _userManager.UpdateAsync(user);

                // Check if the update was successful
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success", Message = "User updated successfully." });
                }
                else
                {
                    // Return error response if the update fails
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Failed to update user." });
                }
            }
            catch (Exception ex)
            {


                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "An error occurred while updating the user." });
            }
        }

    }
}
