using GPMS.Core.Models.Authentication.SignUp;
using GPMS.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GPMS.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GPMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager,
             RoleManager<IdentityRole> roleManager, IEmailService emailService,
             SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role)
        {
            //Check User Exist 
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new Response { Status = "Error", Message = "User already exists!" });
            }

            //Add the User in the database
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
                //Add role to the user....

                await _userManager.AddToRoleAsync(user, role);

                //Add Token to Verify the email....
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                _emailService.SendEmail(message);



                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User created & Email Sent to {user.Email} SuccessFully" });

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This Role Doesnot Exist." });
            }


        }
        [HttpGet("GetAllUsers")]  // Define your route for the GET request, for example, "api/auth/GetAllUsers"
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Retrieve all users or any information you want to expose
                var allUsers = await _userManager.Users.ToListAsync();  // Example: fetching all users

                // Return the list of users or relevant information
                return Ok(allUsers);
            }
            catch (Exception ex)
            {
             
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "An error occurred while fetching users." });
            }
        }


        [HttpDelete("DeleteUser/{userId}")]  // Define your route for the DELETE request, for example, "api/auth/DeleteUser/{userId}"
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                // Find the user by userId
                var user = await _userManager.FindByIdAsync(userId);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound(new Response { Status = "Error", Message = "User not found." });
                }

                // Delete the user from the database
                var result = await _userManager.DeleteAsync(user);

                // Check if the deletion was successful
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success", Message = "User deleted successfully." });
                }
                else
                {
                    // Return error response if the deletion fails
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "Failed to delete user." });
                }
            }
            catch (Exception ex)
            {

                // Return an error response
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

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                      new Response { Status = "Success", Message = "Email Verified Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response { Status = "Error", Message = "This User Doesnot exist!" });
        }




        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

    }
}
