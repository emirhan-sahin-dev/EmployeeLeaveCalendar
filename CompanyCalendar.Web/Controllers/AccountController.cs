using Microsoft.AspNetCore.Mvc;
using CompanyCalendar.Infrastructure.Identity;
using CompanyCalendar.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CompanyCalendar.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(
            model.Email.Trim());

        if (user is null || !user.IsActive)
        {
            ModelState.AddModelError(
                string.Empty,
                "E-posta adresi veya parola hatalıdır.");

            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            user.LastLoginAt = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            _logger.LogInformation(
                "{Email} kullanıcısı sisteme giriş yaptı.",
                user.Email);

            if (IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl!);
            }

            return RedirectToAction("Index", "Home");
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "Çok fazla hatalı giriş yapıldı. Hesabınız geçici olarak kilitlendi.");

            return View(model);
        }

        ModelState.AddModelError(
            string.Empty,
            "E-posta adresi veya parola hatalıdır.");

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private bool IsLocalUrl(string? returnUrl)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) &&
               Url.IsLocalUrl(returnUrl);
    }
}
