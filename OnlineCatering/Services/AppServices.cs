using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Models;
using OnlineCatering.ViewModels;

namespace OnlineCatering.Services;

public class SessionKeys
{
    public const string UserId = "UserId";
    public const string UserType = "UserType";
    public const string ReferenceId = "ReferenceId";
    public const string DisplayName = "DisplayName";
}

public class BookingService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;

    public BookingService(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public int MinAdvanceDays => _config.GetValue("BusinessRules:MinAdvanceBookingDays", 7);
    public int MinGuestCount => _config.GetValue("BusinessRules:MinGuestCount", 50);

    public (bool IsValid, string Error) ValidateBooking(DateTime eventDate, int guestCount)
    {
        if (guestCount < MinGuestCount)
            return (false, $"Minimum booking size is {MinGuestCount} people.");

        if (eventDate.Date < DateTime.Today.AddDays(MinAdvanceDays))
            return (false, $"Booking must be made at least {MinAdvanceDays} days before the event.");

        return (true, string.Empty);
    }

    public string GenerateBookingId()
    {
        return $"BK{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }

    public string GenerateInvoiceNumber()
    {
        return $"INV{DateTime.Now:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }

    public async Task<CustomerInvoice> GenerateInvoiceAsync(int orderId)
    {
        var order = await _db.CustOrders
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order == null)
            throw new InvalidOperationException("Order not found.");

        if (await _db.CustomerInvoices.AnyAsync(i => i.OrderId == orderId))
            throw new InvalidOperationException("Invoice already exists.");

        var subTotal = order.TotalAmount;
        var tax = Math.Round(subTotal * 0.05m, 2);
        var grandTotal = subTotal + tax;

        var invoice = new CustomerInvoice
        {
            InvoiceNumber = GenerateInvoiceNumber(),
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CatererId = order.CatererId,
            SubTotal = subTotal,
            TaxAmount = tax,
            GrandTotal = grandTotal
        };

        foreach (var item in order.OrderItems)
        {
            invoice.InvoiceItems.Add(new CustomerInvoiceChild
            {
                ItemName = item.MenuItem?.ItemName ?? "Menu Item",
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            });
        }

        _db.CustomerInvoices.Add(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public decimal CalculateCancellationCharge(Caterer caterer, decimal totalAmount)
    {
        return Math.Round(totalAmount * caterer.CancellationChargePercent / 100m, 2);
    }
}

public class AuthService
{
    private readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db) => _db = db;

    public async Task<LoginMaster?> ValidateLoginAsync(string username, string password, string userType)
    {
        return await _db.LoginMaster.FirstOrDefaultAsync(l =>
            l.Username == username &&
            l.Password == password &&
            l.UserType == userType &&
            l.IsActive);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _db.LoginMaster.AnyAsync(l => l.Username == username);
    }
}
