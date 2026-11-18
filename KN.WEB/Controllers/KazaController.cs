using System.Security.Claims;
using KN.Core.Enums;
using KN.Core.Models;
using KN.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KN.WEB.Controllers;

[Authorize]
public class KazaController(IKazaService kaza) : Controller
{
    private readonly IKazaService _kaza = kaza;
    
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPeriod(DateOnly start, DateOnly end)
        => Done(await _kaza.AddPeriodAsync(UserId, start, end), "Период добавлен.");
    
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePeriod(int id)
        => Done(await _kaza.DeletePeriodAsync(UserId, id), "Период удалён.");

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRead(PrayerType prayer, int count)
        => Done(await _kaza.AddReadAsync(UserId, prayer, count), $"{prayer.ToDisplayName()}: отмечено {count}.");

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLog(int id)
        => Done(await _kaza.DeleteLogAsync(UserId, id), "Запись удалена, остаток восстановлен.");

    private IActionResult Done(OperationResult result, string successMessage)
    {
        if (result.Ok) TempData["Success"] = successMessage;
        else TempData["Error"] = result.Error;
        return RedirectToAction("Index", "Home");
    }
}