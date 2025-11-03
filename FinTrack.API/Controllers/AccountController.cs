using FinTrack.API.DTOs.Account;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Utility.Service;

namespace FinTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Email already in use." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PreferredCurrency = model.PreferredCurrency,
                MonthlyIncome = model.MonthlyIncome
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmLink = Url.Action(
                "confirm-email",
                "account",
                new { userId = user.Id, token },
                Request.Scheme);

            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your FinTrack account",
                $"Hello {user.FirstName}, please confirm your account by clicking <a href='{confirmLink}'>here</a>."
            );

            return Ok(new { message = "Registration successful. Check your email to confirm your account." });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Invalid confirmation parameters." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { message = "User not found." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Email confirmed successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return Unauthorized(new { message = "Invalid email or password." });

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new { message = "Please confirm your email first." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new
            {
                token = await _tokenService.CreateTokenAsync(user),
                user = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PreferredCurrency = user.PreferredCurrency,
                    MonthlyIncome = user.MonthlyIncome
                }
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return Ok(new { message = "If the email exists, a reset link has been sent." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("reset-password", "account", new { token, email = user.Email }, Request.Scheme);

            var emailBody = $@"
                <h3>Hello {user.FirstName},</h3>
                <p>You can reset your <strong>FinTrack</strong> password by clicking the link below:</p>
                <p><a href='{resetLink}' 
                      style='color:#007bff;text-decoration:none;font-weight:bold;'>Reset your password</a></p>
                <p>If you didn’t request this, please ignore this email.</p>
            ";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset your FinTrack password",
                emailBody
            );

            return Ok(new { message = "If your email is registered, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest(new { message = "Invalid request." });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(new { message = "Password has been reset successfully." });
        }

        
        [HttpGet("me")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = "e650f77b-f9b3-41c2-b833-500e35dd2c1b";
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PreferredCurrency = user.PreferredCurrency,
                MonthlyIncome = user.MonthlyIncome
            });
        }
    }
}
