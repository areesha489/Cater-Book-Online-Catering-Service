namespace OnlineCatering.Models;

public class MenuCategory
{
    public int CategoryId { get; set; }
    public int CatererId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Caterer? Caterer { get; set; }
    public ICollection<Menu> MenuItems { get; set; } = new List<Menu>();
}
