using ITHelpDesk.Application.DTOs.Authentications;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ITHelpDesk.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
            => _authService = authService;

        // GET /Account/Login
        public IActionResult Login() => View(new LoginDto());

        // POST /Account/Login 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
            {
                ModelState.AddValidationErrors(result.Errors);
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Index", "Tickets");
        }

        // GET /Account/Register
        public IActionResult Register() => View(new RegisterDto());

        // POST /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var confirmationBaseUrl = Url.Action("ConfirmEmail", "Account", null, Request.Scheme);

            var result = await _authService.RegisterAsync(dto, confirmationBaseUrl);
            if (!result.Success)
            {
                ModelState.AddValidationErrors(result.Errors);
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction("Login");
        }

        // POST /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);

            if (!result)
                return View("EmailConfirmationFailed");

            return View("EmailConfirmation");
        }

        // GET /Account/AccessDenied
        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }
    }
}
