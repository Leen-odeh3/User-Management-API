
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using GPMS.Core.Interfaces;
using GPMS.Core.Models.Authentication.Login;
using GPMS.Core.Models.Authentication.SignUp;
using GPMS.Core.Models;


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
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


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }


                var jwtToken = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });

            }
            return Unauthorized();


        }

        [HttpPost]
        [Route("login-2FA")] //Two Factor Auth.
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var jwtToken = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });


                }
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = "Success", Message = $"Invalid Code" });
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

        [HttpPost]
        [Route("forgot-password")]
        [AllowAnonymous] //  doesn't require authorization
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                     new Response
                     { Status = "Error", Message = $"Couldn't send link to the specified email: ${email}" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // The link once clicked will lead the user to the action method whose name is `ResetPassword`
            var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);

            // Specify the email recepient(s) and prepare the email subject and content
            var message = new Message(new string[] { user.Email! }, "Forgot Password Link", forgotPasswordLink!);


            _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"Reset Forgotten Password Request has been sent to email: {user.Email}" }); ;
        }


        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPassword { Token = token, Email = email };

            return Ok(model);
        }

        [HttpPost]
        [Route("reset-password")]
        [AllowAnonymous] // means that this method doesn't require authorization
        public async Task<IActionResult> ResetPassword(ResetPassword resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                     new Response
                     { Status = "Error", Message = $"Couldn't find a user with the specified email: ${resetPasswordDto.Email}" });
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

            if (!resetPasswordResult.Succeeded)
            {
                resetPasswordResult.Errors.ToList().ForEach(error =>
                ModelState.AddModelError(error.Code, error.Description));

                return BadRequest(ModelState);
            }

            return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"Password has successfully been reset for account with email: {user.Email}" }); ;
        }




    }
}