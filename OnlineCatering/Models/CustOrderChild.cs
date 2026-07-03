namespace OnlineCatering.Models;

public class CustOrderChild
{
    public int OrderChildId { get; set; }
    public int OrderId { get; set; }
    public int MenuId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public CustOrder? Order { get; set; }
    public Menu? MenuItem { get; set; }
}
