using ITHelpDesk.Application;
using ITHelpDesk.Application.DTOs.Authentications;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ITHelpDesk.Infrastructure.Services.Identity
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<GeneralResult> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return GeneralResult.FailedResult("Invalid email or password.");

            var result = await _signInManager.PasswordSignInAsync(
                user, dto.Password, dto.RememberMe, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);

                var remainingTime = lockoutEnd.HasValue
                    ? (lockoutEnd.Value - DateTimeOffset.UtcNow).TotalSeconds
                    : 0;

                return GeneralResult.FailedResult(
                    $"Account locked. Try again after {Math.Ceiling(remainingTime)} seconds");
            }

            if (result.IsNotAllowed)
            {
                return GeneralResult.FailedResult("There is a confirmed email sent to you\nPlease confirm your email first.");
            }

            if (!result.Succeeded)
            {
                return GeneralResult.FailedResult("Invalid Email or Password");
            }

            return GeneralResult.SuccessedResult("User logged in successfully");
        }

        public async Task<GeneralResult> RegisterAsync(RegisterDto dto, string confirmationBaseUrl)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return GeneralResult.FailedResult("Email is already registered.");

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return GeneralResult.FailedResult($"User registration failed: " + string.Join("\n", errors));
            }

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addRoleResult.Succeeded)
            {
                return GeneralResult.FailedResult("Role assignment failed");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{confirmationBaseUrl}?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", $"<a href='{confirmationLink}'>Confirm Email</a>");

            return GeneralResult.SuccessedResult("Registered successfully.");
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task LogoutAsync()
            => await _signInManager.SignOutAsync();
    }
}