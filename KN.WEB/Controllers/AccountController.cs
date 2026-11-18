using KN.Core.Constants;
using KN.Core.Entities;
using KN.WEB.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KN.WEB.Controllers;

public class AccountController(UserManager<AppUser> users, SignInManager<AppUser> signIn) : Controller
{
    private readonly UserManager<AppUser> _users = users;
    private readonly SignInManager<AppUser> _signIn = signIn;
    
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new AppUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
        var result = await _users.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _users.AddToRoleAsync(user, Roles.User);
            await _signIn.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var e in result.Errors)
            ModelState.AddModelError("", e.Description);
        return   View(model);
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var result = await _signIn.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl)
                : RedirectToAction("Index", "Home");
        
        ModelState.AddModelError("", result.IsLockedOut ? "Аккаунт временно заблокирован. Попробуйте позже." : "Неверный email или пароль.");
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    public IActionResult AccessDenied() => View();
}