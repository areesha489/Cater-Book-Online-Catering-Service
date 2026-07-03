using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Filters;
using OnlineCatering.Models;
using OnlineCatering.Services;
using OnlineCatering.ViewModels;

namespace OnlineCatering.Controllers;

public class CustomerController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly BookingService _booking;

    public CustomerController(ApplicationDbContext db, BookingService booking)
    {
        _db = db;
        _booking = booking;
    }

    private int CustomerId => HttpContext.Session.GetInt32(SessionKeys.ReferenceId) ?? 0;

    [RequireLogin(RequiredUserType = "Customer")]
    public async Task<IActionResult> Dashboard()
    {
        var orders = await _db.CustOrders
            .Include(o => o.Caterer)
            .Where(o => o.CustomerId == CustomerId)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync();

        ViewBag.OrderCount = await _db.CustOrders.CountAsync(o => o.CustomerId == CustomerId);
        ViewBag.FavoriteCount = await _db.FavoriteCaterers.CountAsync(f => f.CustomerId == CustomerId);
        return View(orders);
    }

    public async Task<IActionResult> Search(CatererSearchViewModel? model)
    {
        model ??= new CatererSearchViewModel();
        var query = _db.Caterers.Where(c => c.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.City))
            query = query.Where(c => c.City == model.City);

        if (!string.IsNullOrWhiteSpace(model.Area))
            query = query.Where(c => c.Area.Contains(model.Area) || c.Address.Contains(model.Area));

        if (!string.IsNullOrWhiteSpace(model.FoodType) && model.FoodType != "All")
            query = query.Where(c => c.FoodType == model.FoodType || c.FoodType == "Both");

        if (model.GuestCount.HasValue && model.GuestCount > 0)
            query = query.Where(c => c.MinGuests <= model.GuestCount && c.MaxGuests >= model.GuestCount);

        if (!string.IsNullOrWhiteSpace(model.SearchTerm))
            query = query.Where(c => c.BusinessName.Contains(model.SearchTerm) || c.Description.Contains(model.SearchTerm));

        var results = await query.OrderByDescending(c => c.Rating).ToListAsync();
        ViewBag.Search = model;
        ViewBag.Cities = AreaData.AllCities;
        ViewBag.Areas = AreaData.GetAreas(model.City);
        ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(AreaData.AreasByCity);
        return View(results);
    }

    public async Task<IActionResult> Restaurants(CatererBrowseViewModel? model)
    {
        model ??= new CatererBrowseViewModel();
        var query = _db.Caterers.Where(c => c.IsActive).AsQueryable();

        if (!string.IsNullOrWhiteSpace(model.City))
            query = query.Where(c => c.City == model.City);

        if (!string.IsNullOrWhiteSpace(model.Area))
            query = query.Where(c => c.Area.Contains(model.Area));

        if (!string.IsNullOrWhiteSpace(model.FoodType) && model.FoodType != "All")
            query = query.Where(c => c.FoodType == model.FoodType || c.FoodType == "Both");

        query = model.SortBy switch
        {
            "Name" => query.OrderBy(c => c.BusinessName),
            "City" => query.OrderBy(c => c.City).ThenByDescending(c => c.Rating),
            _ => query.OrderByDescending(c => c.Rating)
        };

        var results = await query.ToListAsync();
        ViewBag.Browse = model;
        ViewBag.Cities = AreaData.AllCities;
        ViewBag.Areas = AreaData.GetAreas(model.City);
        ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(AreaData.AreasByCity);
        ViewBag.TotalCount = await _db.Caterers.CountAsync(c => c.IsActive);
        return View(results);
    }

    public async Task<IActionResult> CatererDetails(int id)
    {
        var caterer = await _db.Caterers
            .Include(c => c.MenuCategories)
            .ThenInclude(mc => mc.MenuItems.Where(m => m.IsAvailable))
            .FirstOrDefaultAsync(c => c.CatererId == id && c.IsActive);

        if (caterer == null) return NotFound();

        if (HttpContext.Session.GetString(SessionKeys.UserType) == "Customer")
        {
            ViewBag.IsFavorite = await _db.FavoriteCaterers
                .AnyAsync(f => f.CustomerId == CustomerId && f.CatererId == id);
        }

        return View(caterer);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavorite(int catererId)
    {
        var existing = await _db.FavoriteCaterers
            .FirstOrDefaultAsync(f => f.CustomerId == CustomerId && f.CatererId == catererId);

        if (existing != null)
            _db.FavoriteCaterers.Remove(existing);
        else
            _db.FavoriteCaterers.Add(new FavoriteCaterer { CustomerId = CustomerId, CatererId = catererId });

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(CatererDetails), new { id = catererId });
    }

    [RequireLogin(RequiredUserType = "Customer")]
    public async Task<IActionResult> Favorites()
    {
        var favorites = await _db.FavoriteCaterers
            .Include(f => f.Caterer)
            .Where(f => f.CustomerId == CustomerId)
            .ToListAsync();
        return View(favorites);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpGet]
    public async Task<IActionResult> Book(int catererId)
    {
        var caterer = await _db.Caterers
            .Include(c => c.MenuCategories)
            .ThenInclude(mc => mc.MenuItems.Where(m => m.IsAvailable))
            .FirstOrDefaultAsync(c => c.CatererId == catererId);

        if (caterer == null) return NotFound();

        var model = new BookingViewModel
        {
            CatererId = caterer.CatererId,
            CatererName = caterer.BusinessName,
            SelectedItems = caterer.MenuCategories
                .SelectMany(mc => mc.MenuItems.Select(m => new MenuSelectionItem
                {
                    MenuId = m.MenuId,
                    ItemName = m.ItemName,
                    PricePerPlate = m.PricePerPlate,
                    CategoryId = mc.CategoryId,
                    CategoryName = mc.CategoryName
                }))
                .ToList()
        };

        ViewBag.MinAdvanceDays = _booking.MinAdvanceDays;
        ViewBag.MinGuestCount = _booking.MinGuestCount;
        return View(model);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookingViewModel model)
    {
        var validation = _booking.ValidateBooking(model.EventDate, model.GuestCount);
        if (!validation.IsValid)
        {
            ModelState.AddModelError(string.Empty, validation.Error);
        }

        var selected = model.SelectedItems?.Where(i => i.IsSelected).ToList() ?? new();
        if (!selected.Any())
            ModelState.AddModelError(string.Empty, "Please select at least one menu item.");

        if (!ModelState.IsValid)
        {
            var caterer = await _db.Caterers.FindAsync(model.CatererId);
            model.CatererName = caterer?.BusinessName ?? "";
            ViewBag.MinAdvanceDays = _booking.MinAdvanceDays;
            ViewBag.MinGuestCount = _booking.MinGuestCount;
            return View(model);
        }

        decimal total = 0;
        var order = new CustOrder
        {
            BookingId = _booking.GenerateBookingId(),
            CustomerId = CustomerId,
            CatererId = model.CatererId,
            EventDate = model.EventDate,
            EventType = model.EventType,
            EventVenue = model.EventVenue,
            GuestCount = model.GuestCount,
            SpecialInstructions = model.SpecialInstructions,
            OrderStatus = "Pending",
            PaymentStatus = "Pending"
        };

        foreach (var item in selected)
        {
            var lineTotal = item.PricePerPlate * model.GuestCount;
            total += lineTotal;
            order.OrderItems.Add(new CustOrderChild
            {
                MenuId = item.MenuId,
                Quantity = model.GuestCount,
                UnitPrice = item.PricePerPlate,
                LineTotal = lineTotal
            });
        }

        order.TotalAmount = total;
        _db.CustOrders.Add(order);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Payment), new { orderId = order.OrderId });
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpGet]
    public async Task<IActionResult> Payment(int orderId)
    {
        var order = await _db.CustOrders
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == CustomerId);

        if (order == null) return NotFound();
        if (order.PaymentStatus == "Paid")
            return RedirectToAction(nameof(BookingDetails), new { id = orderId });

        return View(new PaymentViewModel
        {
            OrderId = order.OrderId,
            BookingId = order.BookingId,
            TotalAmount = order.TotalAmount
        });
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Payment(PaymentViewModel model)
    {
        var order = await _db.CustOrders
            .FirstOrDefaultAsync(o => o.OrderId == model.OrderId && o.CustomerId == CustomerId);

        if (order == null) return NotFound();

        if (model.PaymentMethod is "Easypaisa" or "JazzCash")
        {
            if (string.IsNullOrWhiteSpace(model.MobileNumber))
                ModelState.AddModelError(nameof(model.MobileNumber), "Mobile number is required.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(model.MobileNumber.Trim(), @"^03\d{9}$"))
                ModelState.AddModelError(nameof(model.MobileNumber), "Enter valid Pakistani mobile (03XXXXXXXXX).");
        }
        else if (model.PaymentMethod == "BankAccount")
        {
            if (string.IsNullOrWhiteSpace(model.BankName))
                ModelState.AddModelError(nameof(model.BankName), "Bank name is required.");
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
                ModelState.AddModelError(nameof(model.AccountNumber), "Account number is required.");
            if (string.IsNullOrWhiteSpace(model.AccountTitle))
                ModelState.AddModelError(nameof(model.AccountTitle), "Account title is required.");
        }

        if (!ModelState.IsValid)
            return View(model);

        order.PaymentMethod = model.PaymentMethod switch
        {
            "Easypaisa" => "Easypaisa",
            "JazzCash" => "JazzCash",
            _ => "Bank Account"
        };
        order.PaymentReference = model.PaymentMethod == "BankAccount"
            ? $"{model.BankName} | {model.AccountNumber} | {model.AccountTitle}"
            : model.MobileNumber!.Trim();

        order.PaymentStatus = "Paid";
        order.PaidAmount = order.TotalAmount;
        order.OrderStatus = "Confirmed";
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Payment via {order.PaymentMethod} successful! Booking ID: {order.BookingId}";
        return RedirectToAction(nameof(BookingDetails), new { id = order.OrderId });
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var customer = await _db.Customers.FindAsync(CustomerId);
        if (customer == null) return NotFound();
        return View(customer);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(Customer model)
    {
        var customer = await _db.Customers.FindAsync(CustomerId);
        if (customer == null) return NotFound();

        customer.FullName = model.FullName;
        customer.Email = model.Email;
        customer.Phone = model.Phone;
        customer.Mobile = model.Mobile;
        customer.Address = model.Address;
        customer.City = model.City;
        customer.PinCode = model.PinCode;
        await _db.SaveChangesAsync();

        HttpContext.Session.SetString(SessionKeys.DisplayName, customer.FullName);
        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProfile()
    {
        var customer = await _db.Customers.FindAsync(CustomerId);
        if (customer == null) return NotFound();

        var login = await _db.LoginMaster.FirstOrDefaultAsync(l => l.UserType == "Customer" && l.ReferenceId == CustomerId);
        if (login != null) _db.LoginMaster.Remove(login);
        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();

        HttpContext.Session.Clear();
        TempData["Success"] = "Your profile has been deleted.";
        return RedirectToAction("Index", "Home");
    }

    [RequireLogin(RequiredUserType = "Customer")]
    public async Task<IActionResult> MessageDetails(int id)
    {
        var msg = await _db.Messages.FindAsync(id);
        if (msg == null) return NotFound();
        if (!((msg.SenderId == CustomerId && msg.SenderType == "Customer") ||
              (msg.ReceiverId == CustomerId && msg.ReceiverType == "Customer")))
            return NotFound();

        if (msg.ReceiverId == CustomerId && msg.ReceiverType == "Customer")
        {
            msg.IsRead = true;
            await _db.SaveChangesAsync();
        }
        return View(msg);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpGet]
    public async Task<IActionResult> ReplyMessage(int id)
    {
        var msg = await _db.Messages.FindAsync(id);
        if (msg == null) return NotFound();

        var receiverId = msg.SenderId == CustomerId ? msg.ReceiverId : msg.SenderId;
        var receiverType = msg.SenderId == CustomerId ? msg.ReceiverType : msg.SenderType;
        var receiverName = receiverType == "Caterer"
            ? (await _db.Caterers.FindAsync(receiverId))?.BusinessName ?? "Caterer"
            : "User";

        return View("SendMessage", new MessageViewModel
        {
            ReceiverId = receiverId,
            ReceiverType = receiverType,
            ReceiverName = receiverName,
            Subject = msg.Subject.StartsWith("Re:") ? msg.Subject : "Re: " + msg.Subject,
            Body = ""
        });
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var msg = await _db.Messages.FindAsync(id);
        if (msg != null && ((msg.SenderId == CustomerId && msg.SenderType == "Customer") ||
            (msg.ReceiverId == CustomerId && msg.ReceiverType == "Customer")))
        {
            _db.Messages.Remove(msg);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Message deleted.";
        }
        return RedirectToAction(nameof(Messages));
    }

    [RequireLogin(RequiredUserType = "Customer")]
    public async Task<IActionResult> MyBookings()
    {
        var orders = await _db.CustOrders
            .Include(o => o.Caterer)
            .Where(o => o.CustomerId == CustomerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    public async Task<IActionResult> BookingDetails(int id)
    {
        var order = await _db.CustOrders
            .Include(o => o.Caterer)
            .Include(o => o.OrderItems).ThenInclude(i => i.MenuItem)
            .Include(o => o.Invoice).ThenInclude(i => i!.InvoiceItems)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == CustomerId);

        if (order == null) return NotFound();
        return View(order);
    }

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var order = await _db.CustOrders
            .Include(o => o.Caterer)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == CustomerId);

        if (order == null) return NotFound();
        if (order.OrderStatus == "Cancelled" || order.OrderStatus == "Completed")
            return RedirectToAction(nameof(BookingDetails), new { id });

        var charge = _booking.CalculateCancellationCharge(order.Caterer!, order.TotalAmount);
        order.OrderStatus = "Cancelled";
        order.CancelledDate = DateTime.Now;
        order.CancellationCharge = charge;
        if (order.PaymentStatus == "Paid")
            order.PaymentStatus = "Refunded";

        await _db.SaveChangesAsync();
        TempData["Success"] = $"Booking cancelled. Cancellation charge: Rs. {charge:N0}";
        return RedirectToAction(nameof(MyBookings));
    }

    [RequireLogin(RequiredUserType = "Customer")]
    public async Task<IActionResult> Messages()
    {
        var messages = await _db.Messages
            .Where(m => (m.SenderId == CustomerId && m.SenderType == "Customer") ||
                        (m.ReceiverId == CustomerId && m.ReceiverType == "Customer"))
            .OrderByDescending(m => m.SentDate)
            .ToListAsync();
        return View(messages);
    }

    [RequireLogin(RequiredUserType = "Customer")]
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

    [RequireLogin(RequiredUserType = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(MessageViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Messages.Add(new Message
        {
            SenderId = CustomerId,
            SenderType = "Customer",
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

