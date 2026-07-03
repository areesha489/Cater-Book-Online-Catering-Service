using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Filters;
using OnlineCatering.Models;
using OnlineCatering.Services;

namespace OnlineCatering.Controllers;

[RequireLogin(RequiredUserType = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Dashboard()
    {
        ViewBag.Customers = await _db.Customers.CountAsync();
        ViewBag.Caterers = await _db.Caterers.CountAsync(c => c.IsActive);
        ViewBag.Bookings = await _db.CustOrders.CountAsync();
        ViewBag.PendingPayments = await _db.CustOrders.CountAsync(o => o.PaymentStatus == "Pending");
        ViewBag.Revenue = await _db.CustOrders.Where(o => o.PaymentStatus == "Paid").SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        ViewBag.EasypaisaCount = await _db.CustOrders.CountAsync(o => o.PaymentMethod == "Easypaisa" && o.PaymentStatus == "Paid");
        ViewBag.JazzCashCount = await _db.CustOrders.CountAsync(o => o.PaymentMethod == "JazzCash" && o.PaymentStatus == "Paid");
        ViewBag.BankCount = await _db.CustOrders.CountAsync(o => o.PaymentMethod == "Bank Account" && o.PaymentStatus == "Paid");
        ViewBag.RecentBookings = await _db.CustOrders
            .Include(o => o.Customer).Include(o => o.Caterer)
            .OrderByDescending(o => o.OrderDate).Take(8).ToListAsync();
        ViewBag.TopAreas = await _db.Caterers
            .Where(c => c.IsActive && c.Area != "")
            .GroupBy(c => new { c.City, c.Area })
            .Select(g => new { g.Key.City, g.Key.Area, Count = g.Count() })
            .OrderByDescending(x => x.Count).Take(6).ToListAsync();
        return View();
    }

    public async Task<IActionResult> Customers()
    {
        return View(await _db.Customers.OrderByDescending(c => c.RegisteredDate).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCustomer(int id)
    {
        var c = await _db.Customers.FindAsync(id);
        if (c != null) { c.IsActive = !c.IsActive; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Customers));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var c = await _db.Customers.FindAsync(id);
        if (c != null)
        {
            var login = await _db.LoginMaster.FirstOrDefaultAsync(l => l.UserType == "Customer" && l.ReferenceId == id);
            if (login != null) _db.LoginMaster.Remove(login);
            _db.Customers.Remove(c);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Customer deleted.";
        }
        return RedirectToAction(nameof(Customers));
    }

    public async Task<IActionResult> Caterers()
    {
        return View(await _db.Caterers.OrderByDescending(c => c.RegisteredDate).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleCaterer(int id)
    {
        var c = await _db.Caterers.FindAsync(id);
        if (c != null) { c.IsActive = !c.IsActive; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Caterers));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCaterer(int id)
    {
        var c = await _db.Caterers.FindAsync(id);
        if (c != null)
        {
            var login = await _db.LoginMaster.FirstOrDefaultAsync(l => l.UserType == "Caterer" && l.ReferenceId == id);
            if (login != null) _db.LoginMaster.Remove(login);
            _db.Caterers.Remove(c);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Caterer deleted.";
        }
        return RedirectToAction(nameof(Caterers));
    }

    public async Task<IActionResult> AllBookings()
    {
        var list = await _db.CustOrders
            .Include(o => o.Customer).Include(o => o.Caterer)
            .OrderByDescending(o => o.OrderDate).ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> BookingDetails(int id)
    {
        var order = await _db.CustOrders
            .Include(o => o.Customer).Include(o => o.Caterer)
            .Include(o => o.OrderItems).ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.OrderId == id);
        if (order == null) return NotFound();
        return View(order);
    }

    public async Task<IActionResult> Payments()
    {
        var orders = await _db.CustOrders
            .Include(o => o.Customer).Include(o => o.Caterer)
            .Where(o => o.PaymentStatus == "Paid")
            .OrderByDescending(o => o.OrderDate).ToListAsync();

        ViewBag.EasypaisaTotal = orders.Where(o => o.PaymentMethod == "Easypaisa").Sum(o => o.TotalAmount);
        ViewBag.JazzCashTotal = orders.Where(o => o.PaymentMethod == "JazzCash").Sum(o => o.TotalAmount);
        ViewBag.BankTotal = orders.Where(o => o.PaymentMethod == "Bank Account").Sum(o => o.TotalAmount);
        return View(orders);
    }

    public async Task<IActionResult> AreasReport()
    {
        var data = await _db.Caterers
            .GroupBy(c => new { c.City, c.Area })
            .Select(g => new AreaReportItem
            {
                City = g.Key.City,
                Area = string.IsNullOrEmpty(g.Key.Area) ? "Not Set" : g.Key.Area,
                CatererCount = g.Count(),
                ActiveCount = g.Count(c => c.IsActive),
                AvgRating = g.Average(c => c.Rating)
            })
            .OrderBy(x => x.City).ThenBy(x => x.Area)
            .ToListAsync();
        return View(data);
    }

    public async Task<IActionResult> Announcements()
    {
        return View(await _db.SiteAnnouncements.OrderByDescending(a => a.CreatedDate).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAnnouncement(SiteAnnouncement model)
    {
        if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Message))
        {
            TempData["Error"] = "Title and message are required.";
            return RedirectToAction(nameof(Announcements));
        }

        if (model.AnnouncementId == 0)
            _db.SiteAnnouncements.Add(model);
        else
        {
            var existing = await _db.SiteAnnouncements.FindAsync(model.AnnouncementId);
            if (existing != null)
            {
                existing.Title = model.Title;
                existing.Message = model.Message;
                existing.IsActive = model.IsActive;
            }
        }
        await _db.SaveChangesAsync();
        TempData["Success"] = "Announcement saved.";
        return RedirectToAction(nameof(Announcements));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAnnouncement(int id)
    {
        var a = await _db.SiteAnnouncements.FindAsync(id);
        if (a != null) { a.IsActive = !a.IsActive; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Announcements));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        var a = await _db.SiteAnnouncements.FindAsync(id);
        if (a != null) { _db.SiteAnnouncements.Remove(a); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Announcement deleted.";
        return RedirectToAction(nameof(Announcements));
    }

    public async Task<IActionResult> LoginUsers()
    {
        return View(await _db.LoginMaster.OrderBy(l => l.UserType).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleLogin(int id)
    {
        var l = await _db.LoginMaster.FindAsync(id);
        if (l != null && l.UserType != "Admin") { l.IsActive = !l.IsActive; await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(LoginUsers));
    }

    public IActionResult Modules() => View();
}

public class AreaReportItem
{
    public string City { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public int CatererCount { get; set; }
    public int ActiveCount { get; set; }
    public decimal AvgRating { get; set; }
}
