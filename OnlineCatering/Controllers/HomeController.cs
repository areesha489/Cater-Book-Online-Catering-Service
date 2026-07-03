using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.ViewModels;

namespace OnlineCatering.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var caterers = await _db.Caterers
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.Rating)
            .Take(6)
            .ToListAsync();
        ViewBag.Announcements = await _db.SiteAnnouncements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedDate)
            .Take(3)
            .ToListAsync();
        ViewBag.Cities = OnlineCatering.Services.AreaData.AllCities;
        ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(OnlineCatering.Services.AreaData.AreasByCity);
        return View(caterers);
    }

    public IActionResult About() => View();

    public IActionResult Services() => View();

    public IActionResult Contact() => View();

    public IActionResult Modules() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
