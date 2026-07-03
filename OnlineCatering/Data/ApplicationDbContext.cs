using Microsoft.EntityFrameworkCore;
using OnlineCatering.Models;

namespace OnlineCatering.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<LoginMaster> LoginMaster => Set<LoginMaster>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Caterer> Caterers => Set<Caterer>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<CustOrder> CustOrders => Set<CustOrder>();
    public DbSet<CustOrderChild> CustOrderChildren => Set<CustOrderChild>();
    public DbSet<CustomerInvoice> CustomerInvoices => Set<CustomerInvoice>();
    public DbSet<CustomerInvoiceChild> CustomerInvoiceChildren => Set<CustomerInvoiceChild>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierOrder> SupplierOrders => Set<SupplierOrder>();
    public DbSet<SupplierOrderChild> SupplierOrderChildren => Set<SupplierOrderChild>();
    public DbSet<SuppInvoice> SuppInvoices => Set<SuppInvoice>();
    public DbSet<SuppInvOrdChild> SuppInvOrdChildren => Set<SuppInvOrdChild>();
    public DbSet<RawMaterial> RawMaterials => Set<RawMaterial>();
    public DbSet<WorkerType> WorkerTypes => Set<WorkerType>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<WorkerSalary> WorkerSalaries => Set<WorkerSalary>();
    public DbSet<FavoriteCaterer> FavoriteCaterers => Set<FavoriteCaterer>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<SiteAnnouncement> SiteAnnouncements => Set<SiteAnnouncement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginMaster>().HasKey(l => l.LoginId);
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<Caterer>().HasKey(c => c.CatererId);
        modelBuilder.Entity<MenuCategory>().HasKey(m => m.CategoryId);
        modelBuilder.Entity<Menu>().HasKey(m => m.MenuId);
        modelBuilder.Entity<CustOrder>().HasKey(c => c.OrderId);
        modelBuilder.Entity<CustOrderChild>().HasKey(c => c.OrderChildId);
        modelBuilder.Entity<CustomerInvoice>().HasKey(c => c.InvoiceId);
        modelBuilder.Entity<CustomerInvoiceChild>().HasKey(c => c.InvoiceChildId);
        modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);
        modelBuilder.Entity<SupplierOrder>().HasKey(s => s.SupplierOrderId);
        modelBuilder.Entity<SupplierOrderChild>().HasKey(s => s.OrderChildId);
        modelBuilder.Entity<SuppInvoice>().HasKey(s => s.SuppInvoiceId);
        modelBuilder.Entity<SuppInvOrdChild>().HasKey(s => s.InvChildId);
        modelBuilder.Entity<RawMaterial>().HasKey(r => r.RawMaterialId);
        modelBuilder.Entity<WorkerType>().HasKey(w => w.WorkerTypeId);
        modelBuilder.Entity<Worker>().HasKey(w => w.WorkerId);
        modelBuilder.Entity<WorkerSalary>().HasKey(w => w.SalaryId);
        modelBuilder.Entity<FavoriteCaterer>().HasKey(f => f.FavoriteId);
        modelBuilder.Entity<Message>().HasKey(m => m.MessageId);
        modelBuilder.Entity<SiteAnnouncement>().HasKey(a => a.AnnouncementId);

        modelBuilder.Entity<LoginMaster>().HasIndex(l => l.Username).IsUnique();
        modelBuilder.Entity<CustOrder>().HasIndex(o => o.BookingId).IsUnique();
        modelBuilder.Entity<CustomerInvoice>().HasIndex(i => i.InvoiceNumber).IsUnique();
        modelBuilder.Entity<SupplierOrder>().HasIndex(o => o.SuppOrderNo).IsUnique();
        modelBuilder.Entity<SuppInvoice>().HasIndex(i => i.InvoiceNo).IsUnique();

        modelBuilder.Entity<CustOrder>()
            .HasOne(o => o.Invoice)
            .WithOne(i => i.Order)
            .HasForeignKey<CustomerInvoice>(i => i.OrderId);

        modelBuilder.Entity<SupplierOrder>()
            .HasOne(o => o.Invoice)
            .WithOne(i => i.Order)
            .HasForeignKey<SuppInvoice>(i => i.SupplierOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FavoriteCaterer>()
            .HasIndex(f => new { f.CustomerId, f.CatererId }).IsUnique();

        modelBuilder.Entity<SupplierOrder>()
            .HasOne(s => s.Caterer).WithMany(c => c.SupplierOrders)
            .HasForeignKey(s => s.CatererId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierOrder>()
            .HasOne(s => s.Supplier).WithMany(s => s.Orders)
            .HasForeignKey(s => s.SupplierId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustOrder>()
            .HasOne(o => o.Customer).WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CustOrder>()
            .HasOne(o => o.Caterer).WithMany(c => c.Orders)
            .HasForeignKey(o => o.CatererId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierOrderChild>()
            .HasOne(c => c.Order).WithMany(o => o.OrderItems)
            .HasForeignKey(c => c.SupplierOrderId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SupplierOrderChild>()
            .HasOne(c => c.RawMaterial).WithMany()
            .HasForeignKey(c => c.RawMaterialId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SuppInvoice>()
            .HasOne(i => i.Supplier).WithMany()
            .HasForeignKey(i => i.SupplierId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SuppInvOrdChild>()
            .HasOne(c => c.Invoice).WithMany(i => i.InvoiceItems)
            .HasForeignKey(c => c.SuppInvoiceId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SuppInvOrdChild>()
            .HasOne(c => c.RawMaterial).WithMany()
            .HasForeignKey(c => c.RawMaterialId).OnDelete(DeleteBehavior.Restrict);
    }
}
