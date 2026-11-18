using KN.Core.Constants;
using KN.Core.Entities;
using KN.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KN.WEB.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(UserManager<AppUser> users) : Controller
{
    private readonly UserManager<AppUser> _users = users;

    public async Task<IActionResult> Index()
    {
        var currentId = _users.GetUserId(User);
        var list = await _users.Users.OrderBy(u => u.Email).ToListAsync();

        var rows = new List<UserRowViewModel>();
        foreach (var u in list)
        {
            var roles = await _users.GetRolesAsync(u);
            rows.Add(new UserRowViewModel
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName,
                CreatedAt =  u.CreatedAt,
                Roles = roles,
                IsAdmin = roles.Contains(Roles.Admin),
                IsCurrent = u.Id == currentId
            });

        }
        return View(rows);
    }
    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null) return NotFound();

        // Нельзя снять админку с самого себя (чтобы не остаться без админов)
        if (user.Id == _users.GetUserId(User))
        {
            TempData["Error"] = "Нельзя изменить свою собственную роль.";
            return RedirectToAction(nameof(Index));
        }

        if (await _users.IsInRoleAsync(user, Roles.Admin))
            await _users.RemoveFromRoleAsync(user, Roles.Admin);
        else
            await _users.AddToRoleAsync(user, Roles.Admin);

        // Чтобы роль применилась у пользователя без ожидания
        await _users.UpdateSecurityStampAsync(user);

        return RedirectToAction(nameof(Index));
    }
}