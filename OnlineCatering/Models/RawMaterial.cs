namespace OnlineCatering.Models;

public class RawMaterial
{
    public int RawMaterialId { get; set; }
    public int CatererId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string Unit { get; set; } = "kg";
    public decimal StockQuantity { get; set; }
    public decimal ReorderLevel { get; set; }

    public Caterer? Caterer { get; set; }
}
