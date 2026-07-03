using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Filters;
using OnlineCatering.Models;
using OnlineCatering.Services;
using OnlineCatering.ViewModels;

namespace OnlineCatering.Controllers;

[RequireLogin(RequiredUserType = "Caterer")]
public class CatererController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly BookingService _booking;
    private readonly ImageUploadService _imageUpload;

    public CatererController(ApplicationDbContext db, BookingService booking, ImageUploadService imageUpload)
    {
        _db = db;
        _booking = booking;
        _imageUpload = imageUpload;
    }

    private int CatererId => HttpContext.Session.GetInt32(SessionKeys.ReferenceId) ?? 0;

    public async Task<IActionResult> Dashboard()
    {
        var caterer = await _db.Caterers.FindAsync(CatererId);
        var pendingOrders = await _db.CustOrders.CountAsync(o => o.CatererId == CatererId && o.OrderStatus == "Pending");
        var confirmedOrders = await _db.CustOrders.CountAsync(o => o.CatererId == CatererId && o.OrderStatus == "Confirmed");
        var totalRevenue = await _db.CustOrders
            .Where(o => o.CatererId == CatererId && o.PaymentStatus == "Paid")
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        ViewBag.Caterer = caterer;
        ViewBag.PendingOrders = pendingOrders;
        ViewBag.ConfirmedOrders = confirmedOrders;
        ViewBag.TotalRevenue = totalRevenue;

        var recent = await _db.CustOrders
            .Include(o => o.Customer)
            .Where(o => o.CatererId == CatererId)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync();

        return View(recent);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var caterer = await _db.Caterers.FindAsync(CatererId);
        if (caterer == null) return NotFound();
        ViewBag.Cities = AreaData.AllCities;
        ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(AreaData.AreasByCity);
        return View(caterer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(Caterer model, IFormFile? restaurantImage)
    {
        var caterer = await _db.Caterers.FindAsync(CatererId);
        if (caterer == null) return NotFound();

        caterer.BusinessName = model.BusinessName;
        caterer.OwnerName = model.OwnerName;
        caterer.Email = model.Email;
        caterer.Phone = model.Phone;
        caterer.Address = model.Address;
        caterer.City = model.City;
        caterer.Area = model.Area;
        caterer.FoodType = model.FoodType;
        caterer.Description = model.Description;
        caterer.CancellationChargePercent = model.CancellationChargePercent;
        caterer.MinGuests = model.MinGuests;
        caterer.MaxGuests = model.MaxGuests;

        if (restaurantImage != null && restaurantImage.Length > 0)
        {
            var upload = await _imageUpload.SaveCatererImageAsync(restaurantImage, CatererId);
            if (!upload.Success)
            {
                ViewBag.Cities = AreaData.AllCities;
                ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(AreaData.AreasByCity);
                TempData["Error"] = upload.Error;
                return View(caterer);
            }
            _imageUpload.DeleteLocalFile(caterer.ImageUrl);
            caterer.ImageUrl = upload.RelativePath;
        }

        await _db.SaveChangesAsync();
        HttpContext.Session.SetString(SessionKeys.DisplayName, caterer.BusinessName);
        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    public async Task<IActionResult> MenuCategories()
    {
        var categories = await _db.MenuCategories
            .Include(c => c.MenuItems)
            .Where(c => c.CatererId == CatererId)
            .ToListAsync();
        return View(categories);
    }

    [HttpGet]
    public IActionResult AddCategory() => View(new MenuCategory { CatererId = CatererId });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(MenuCategory model)
    {
        if (!ModelState.IsValid) return View(model);
        model.CatererId = CatererId;
        _db.MenuCategories.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Category added.";
        return RedirectToAction(nameof(MenuCategories));
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var cat = await _db.MenuCategories.FirstOrDefaultAsync(c => c.CategoryId == id && c.CatererId == CatererId);
        if (cat == null) return NotFound();
        return View(cat);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(MenuCategory model)
    {
        var cat = await _db.MenuCategories.FirstOrDefaultAsync(c => c.CategoryId == model.CategoryId && c.CatererId == CatererId);
        if (cat == null) return NotFound();

        cat.CategoryName = model.CategoryName;
        cat.Description = model.Description;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MenuCategories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var cat = await _db.MenuCategories
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.CategoryId == id && c.CatererId == CatererId);
        if (cat != null)
        {
            _db.Menus.RemoveRange(cat.MenuItems);
            _db.MenuCategories.Remove(cat);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(MenuCategories));
    }

    [HttpGet]
    public async Task<IActionResult> AddMenuItem(int categoryId)
    {
        var cat = await _db.MenuCategories.FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.CatererId == CatererId);
        if (cat == null) return NotFound();
        ViewBag.CategoryName = cat.CategoryName;
        return View(new Menu { CategoryId = categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMenuItem(Menu model)
    {
        var cat = await _db.MenuCategories.FirstOrDefaultAsync(c => c.CategoryId == model.CategoryId && c.CatererId == CatererId);
        if (cat == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.CategoryName = cat.CategoryName;
            return View(model);
        }

        _db.Menus.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MenuCategories));
    }

    [HttpGet]
    public async Task<IActionResult> EditMenuItem(int id)
    {
        var menu = await _db.Menus
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.MenuId == id && m.Category!.CatererId == CatererId);
        if (menu == null) return NotFound();
        return View(menu);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMenuItem(Menu model)
    {
        var menu = await _db.Menus
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.MenuId == model.MenuId && m.Category!.CatererId == CatererId);
        if (menu == null) return NotFound();

        menu.ItemName = model.ItemName;
        menu.Description = model.Description;
        menu.PricePerPlate = model.PricePerPlate;
        menu.IsVeg = model.IsVeg;
        menu.IsAvailable = model.IsAvailable;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(MenuCategories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var menu = await _db.Menus
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.MenuId == id && m.Category!.CatererId == CatererId);
        if (menu != null)
        {
            _db.Menus.Remove(menu);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(MenuCategories));
    }

    public async Task<IActionResult> Bookings()
    {
        var orders = await _db.CustOrders
            .Include(o => o.Customer)
            .Where(o => o.CatererId == CatererId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> BookingDetails(int id)
    {
        var order = await _db.CustOrders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems).ThenInclude(i => i.MenuItem)
            .Include(o => o.Invoice)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CatererId == CatererId);

        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBookingStatus(int id, string status)
    {
        var order = await _db.CustOrders.FirstOrDefaultAsync(o => o.OrderId == id && o.CatererId == CatererId);
        if (order == null) return NotFound();

        order.OrderStatus = status;
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Booking status updated to {status}.";
        return RedirectToAction(nameof(BookingDetails), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfile()
    {
        var caterer = await _db.Caterers.FindAsync(CatererId);
        if (caterer == null) return NotFound();

        var login = await _db.LoginMaster.FirstOrDefaultAsync(l => l.UserType == "Caterer" && l.ReferenceId == CatererId);
        if (login != null) _db.LoginMaster.Remove(login);
        _db.Caterers.Remove(caterer);
        await _db.SaveChangesAsync();

        HttpContext.Session.Clear();
        TempData["Success"] = "Caterer profile deleted.";
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Salaries()
    {
        var salaries = await _db.WorkerSalaries
            .Include(s => s.Worker).ThenInclude(w => w!.WorkerType)
            .Where(s => s.Worker!.CatererId == CatererId)
            .OrderByDescending(s => s.PayYear).ThenByDescending(s => s.PayMonth)
            .ToListAsync();
        ViewBag.Workers = await _db.Workers.Where(w => w.CatererId == CatererId && w.IsActive).ToListAsync();
        return View(salaries);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSalary(WorkerSalary model)
    {
        var worker = await _db.Workers.FirstOrDefaultAsync(w => w.WorkerId == model.WorkerId && w.CatererId == CatererId);
        if (worker == null) return RedirectToAction(nameof(Salaries));

        var wt = await _db.WorkerTypes.FindAsync(worker.WorkerTypeId);
        model.TotalPay = (wt?.PayPerDay ?? 0) * model.WorkingDays;
        _db.WorkerSalaries.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Salary record added.";
        return RedirectToAction(nameof(Salaries));
    }

    public async Task<IActionResult> SupplierOrders()
    {
        var orders = await _db.SupplierOrders
            .Include(o => o.Supplier)
            .Include(o => o.OrderItems).ThenInclude(i => i.RawMaterial)
            .Where(o => o.CatererId == CatererId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        ViewBag.Suppliers = await _db.Suppliers.Where(s => s.CatererId == CatererId).ToListAsync();
        ViewBag.Materials = await _db.RawMaterials.Where(r => r.CatererId == CatererId).ToListAsync();
        return View(orders);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSupplierOrder(int supplierId, int rawMaterialId, decimal quantity, decimal ratePerKg)
    {
        var order = new SupplierOrder
        {
            SuppOrderNo = $"SO{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}",
            CatererId = CatererId,
            SupplierId = supplierId,
            EstimatedAmount = quantity * ratePerKg,
            OrderItems =
            {
                new SupplierOrderChild
                {
                    RawMaterialId = rawMaterialId,
                    Quantity = quantity,
                    RatePerKg = ratePerKg,
                    LineTotal = quantity * ratePerKg
                }
            }
        };
        _db.SupplierOrders.Add(order);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Supplier order created.";
        return RedirectToAction(nameof(SupplierOrders));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateSuppInvoice(int orderId)
    {
        var order = await _db.SupplierOrders
            .Include(o => o.OrderItems).ThenInclude(i => i.RawMaterial)
            .FirstOrDefaultAsync(o => o.SupplierOrderId == orderId && o.CatererId == CatererId);
        if (order == null || order.InvoiceDone) return RedirectToAction(nameof(SupplierOrders));

        var inv = new SuppInvoice
        {
            InvoiceNo = $"SI{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}",
            SupplierId = order.SupplierId,
            SupplierOrderId = order.SupplierOrderId,
            CatererId = CatererId,
            EstimatedAmount = order.EstimatedAmount
        };
        foreach (var item in order.OrderItems)
        {
            inv.InvoiceItems.Add(new SuppInvOrdChild
            {
                RawMaterialId = item.RawMaterialId,
                Quantity = item.Quantity,
                RatePerKg = item.RatePerKg,
                LineTotal = item.LineTotal
            });
        }
        order.InvoiceDone = true;
        _db.SuppInvoices.Add(inv);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Supplier invoice generated.";
        return RedirectToAction(nameof(SuppInvoices));
    }

    public async Task<IActionResult> SuppInvoices()
    {
        var list = await _db.SuppInvoices
            .Include(i => i.Supplier)
            .Include(i => i.Order)
            .Where(i => i.CatererId == CatererId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> MessageDetails(int id)
    {
        var msg = await _db.Messages.FindAsync(id);
        if (msg == null) return NotFound();
        if (!((msg.SenderId == CatererId && msg.SenderType == "Caterer") ||
              (msg.ReceiverId == CatererId && msg.ReceiverType == "Caterer")))
            return NotFound();
        if (msg.ReceiverId == CatererId && msg.ReceiverType == "Caterer")
        {
            msg.IsRead = true;
            await _db.SaveChangesAsync();
        }
        return View(msg);
    }

    [HttpGet]
    public async Task<IActionResult> ReplyMessage(int id)
    {
        var msg = await _db.Messages.FindAsync(id);
        if (msg == null) return NotFound();

        var receiverId = msg.SenderId == CatererId ? msg.ReceiverId : msg.SenderId;
        var receiverType = msg.SenderId == CatererId ? msg.ReceiverType : msg.SenderType;
        var receiverName = receiverType == "Customer"
            ? (await _db.Customers.FindAsync(receiverId))?.FullName ?? "Customer"
            : "User";

        return View("SendMessage", new MessageViewModel
        {
            ReceiverId = receiverId,
            ReceiverType = receiverType,
            ReceiverName = receiverName,
            Subject = msg.Subject.StartsWith("Re:") ? msg.Subject : "Re: " + msg.Subject
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var msg = await _db.Messages.FindAsync(id);
        if (msg != null && ((msg.SenderId == CatererId && msg.SenderType == "Caterer") ||
            (msg.ReceiverId == CatererId && msg.ReceiverType == "Caterer")))
        {
            _db.Messages.Remove(msg);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Messages));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateInvoice(int orderId)
    {
        try
        {
            await _booking.GenerateInvoiceAsync(orderId);
            TempData["Success"] = "Invoice generated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(BookingDetails), new { id = orderId });
    }

    public async Task<IActionResult> Invoices()
    {
        var invoices = await _db.CustomerInvoices
            .Include(i => i.Order).ThenInclude(o => o!.Customer)
            .Where(i => i.CatererId == CatererId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
        return View(invoices);
    }

    public async Task<IActionResult> InvoiceDetails(int id)
    {
        var invoice = await _db.CustomerInvoices
            .Include(i => i.InvoiceItems)
            .Include(i => i.Order).ThenInclude(o => o!.Customer)
            .FirstOrDefaultAsync(i => i.InvoiceId == id && i.CatererId == CatererId);
        if (invoice == null) return NotFound();
        return View(invoice);
    }

    public async Task<IActionResult> Workers()
    {
        var workers = await _db.Workers
            .Include(w => w.WorkerType)
            .Where(w => w.CatererId == CatererId)
            .ToListAsync();
        ViewBag.WorkerTypes = await _db.WorkerTypes.ToListAsync();
        return View(workers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWorker(Worker model)
    {
        model.CatererId = CatererId;
        _db.Workers.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Workers));
    }

    public async Task<IActionResult> RawMaterials()
    {
        var materials = await _db.RawMaterials.Where(r => r.CatererId == CatererId).ToListAsync();
        return View(materials);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRawMaterial(RawMaterial model)
    {
        model.CatererId = CatererId;
        _db.RawMaterials.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(RawMaterials));
    }

    public async Task<IActionResult> Suppliers()
    {
        var suppliers = await _db.Suppliers.Where(s => s.CatererId == CatererId).ToListAsync();
        return View(suppliers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSupplier(Supplier model)
    {
        model.CatererId = CatererId;
        _db.Suppliers.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Suppliers));
    }

    public async Task<IActionResult> Messages()
    {
        var messages = await _db.Messages
            .Where(m => (m.SenderId == CatererId && m.SenderType == "Caterer") ||
                        (m.ReceiverId == CatererId && m.ReceiverType == "Caterer"))
            .OrderByDescending(m => m.SentDate)
            .ToListAsync();
        return View(messages);
    }

    [HttpGet]
    public IActionResult SendMessage(int receiverId, string receiverType, string receiverName)
    {
        return View(new MessageViewModel
        {
            ReceiverId = receiverId,
            ReceiverType = receiverType,
            ReceiverName = receiverName
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(MessageViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Messages.Add(new Message
        {
            SenderId = CatererId,
            SenderType = "Caterer",
            ReceiverId = model.ReceiverId,
            ReceiverType = model.ReceiverType,
            Subject = model.Subject,
            Body = model.Body
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Message sent successfully.";
        return RedirectToAction(nameof(Messages));
    }
}
