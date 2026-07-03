namespace OnlineCatering.Models;

public class CustomerInvoice
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int CatererId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = "Generated"; // Generated, Sent, Paid

    public CustOrder? Order { get; set; }
    public ICollection<CustomerInvoiceChild> InvoiceItems { get; set; } = new List<CustomerInvoiceChild>();
}
