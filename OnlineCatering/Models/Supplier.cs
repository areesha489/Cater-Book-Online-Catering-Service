namespace OnlineCatering.Models;

public class Supplier
{
    public int SupplierId { get; set; }
    public int CatererId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;

    public Caterer? Caterer { get; set; }
    public ICollection<SupplierOrder> Orders { get; set; } = new List<SupplierOrder>();
}
