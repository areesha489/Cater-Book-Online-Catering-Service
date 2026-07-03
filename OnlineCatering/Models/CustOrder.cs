namespace OnlineCatering.Models;

public class CustOrder
{
    public int OrderId { get; set; }
    public string BookingId { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int CatererId { get; set; }
    public DateTime EventDate { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventVenue { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Refunded
    public string PaymentMethod { get; set; } = string.Empty; // Easypaisa, JazzCash, BankAccount
    public string PaymentReference { get; set; } = string.Empty; // mobile or account number
    public string OrderStatus { get; set; } = "Pending"; // Pending, Confirmed, In Progress, Completed, Cancelled
    public string? SpecialInstructions { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? CancelledDate { get; set; }
    public decimal? CancellationCharge { get; set; }

    public Customer? Customer { get; set; }
    public Caterer? Caterer { get; set; }
    public ICollection<CustOrderChild> OrderItems { get; set; } = new List<CustOrderChild>();
    public CustomerInvoice? Invoice { get; set; }
}
