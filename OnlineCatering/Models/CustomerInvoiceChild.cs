namespace OnlineCatering.Models;

public class CustomerInvoiceChild
{
    public int InvoiceChildId { get; set; }
    public int InvoiceId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public CustomerInvoice? Invoice { get; set; }
}
