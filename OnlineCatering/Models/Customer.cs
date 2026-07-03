namespace OnlineCatering.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
    public DateTime RegisteredDate { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;

    public ICollection<CustOrder> Orders { get; set; } = new List<CustOrder>();
    public ICollection<FavoriteCaterer> Favorites { get; set; } = new List<FavoriteCaterer>();
}
