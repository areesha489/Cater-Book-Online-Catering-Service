namespace OnlineCatering.Models;

public class Menu
{
    public int MenuId { get; set; }
    public int CategoryId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerPlate { get; set; }
    public bool IsVeg { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public string ImageUrl { get; set; } = string.Empty;

    public MenuCategory? Category { get; set; }
    public ICollection<CustOrderChild> OrderItems { get; set; } = new List<CustOrderChild>();
}
