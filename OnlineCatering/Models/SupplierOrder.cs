namespace OnlineCatering.Models;

public class SupplierOrder
{
    public int SupplierOrderId { get; set; }
    public string SuppOrderNo { get; set; } = string.Empty;
    public int CatererId { get; set; }
    public int SupplierId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public decimal EstimatedAmount { get; set; }
    public bool InvoiceDone { get; set; }
    public string Status { get; set; } = "Ordered";

    public Caterer? Caterer { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<SupplierOrderChild> OrderItems { get; set; } = new List<SupplierOrderChild>();
    public SuppInvoice? Invoice { get; set; }
}

public class SupplierOrderChild
{
    public int OrderChildId { get; set; }
    public int SupplierOrderId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RatePerKg { get; set; }
    public decimal LineTotal { get; set; }

    public SupplierOrder? Order { get; set; }
    public RawMaterial? RawMaterial { get; set; }
}

public class SuppInvoice
{
    public int SuppInvoiceId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public int SupplierId { get; set; }
    public int SupplierOrderId { get; set; }
    public int CatererId { get; set; }
    public decimal EstimatedAmount { get; set; }

    public Supplier? Supplier { get; set; }
    public SupplierOrder? Order { get; set; }
    public ICollection<SuppInvOrdChild> InvoiceItems { get; set; } = new List<SuppInvOrdChild>();
}

public class SuppInvOrdChild
{
    public int InvChildId { get; set; }
    public int SuppInvoiceId { get; set; }
    public int RawMaterialId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RatePerKg { get; set; }
    public decimal LineTotal { get; set; }

    public SuppInvoice? Invoice { get; set; }
    public RawMaterial? RawMaterial { get; set; }
}
