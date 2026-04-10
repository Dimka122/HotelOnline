using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model, string returnUrl = null)
        {
            _logger.LogInformation("Login attempt for: {Email}", model.Email);
            
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Заполните все поля");
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Неверный email или пароль");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                isPersistent: false, 
                lockoutOnFailure: false);

            _logger.LogInformation("Login result: {Succeeded}, IsLockedOut: {IsLockedOut}", result.Succeeded, result.IsLockedOut);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Аккаунт заблокирован");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Неверный email или пароль");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают");
                return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Client");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}