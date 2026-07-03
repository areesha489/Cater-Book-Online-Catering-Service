namespace OnlineCatering.Models;

public class FavoriteCaterer
{
    public int FavoriteId { get; set; }
    public int CustomerId { get; set; }
    public int CatererId { get; set; }
    public DateTime AddedDate { get; set; } = DateTime.Now;

    public Customer? Customer { get; set; }
    public Caterer? Caterer { get; set; }
}
