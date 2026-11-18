using System.Diagnostics;
using System.Security.Claims;
using KN.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using KN.WEB.Models;

namespace KN.WEB.Controllers;

public class HomeController(IKazaService kaza) : Controller
{
    private readonly IKazaService _kaza = kaza;

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true)
            return View(model: null);
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return View(await _kaza.GetSummaryAsync(userId));
    }

    // public IActionResult Privacy()
    // {
    //     return View();
    // }
    //
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}