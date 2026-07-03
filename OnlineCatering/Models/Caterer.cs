namespace OnlineCatering.Models;

public class Caterer
{
    public int CatererId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string FoodType { get; set; } = string.Empty; // Veg, Non-Veg, Both
    public int MinGuests { get; set; } = 50;
    public int MaxGuests { get; set; } = 5000;
    public decimal CancellationChargePercent { get; set; } = 10;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Rating { get; set; } = 4.0m;
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredDate { get; set; } = DateTime.Now;

    public ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
    public ICollection<CustOrder> Orders { get; set; } = new List<CustOrder>();
    public ICollection<FavoriteCaterer> FavoritedBy { get; set; } = new List<FavoriteCaterer>();
    public ICollection<Worker> Workers { get; set; } = new List<Worker>();
    public ICollection<SupplierOrder> SupplierOrders { get; set; } = new List<SupplierOrder>();
}
