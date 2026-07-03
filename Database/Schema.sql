-- Online Catering System - Database Schema Reference
-- ASP.NET Core uses Entity Framework to create this automatically.
-- Run the application once and the database will be created via EnsureCreated().

/*
TABLES:
- LoginMaster          : Login credentials (Customer/Caterer)
- Customers            : Customer details
- Caterers             : Caterer business profiles
- MenuCategories       : Menu categories per caterer
- Menus                : Food items
- CustOrders           : Customer bookings/orders
- CustOrderChildren    : Selected menu items per order
- CustomerInvoices     : Generated invoices
- CustomerInvoiceChildren : Invoice line items
- Suppliers            : Raw material suppliers
- SupplierOrders       : Supplier purchase orders
- RawMaterials         : Ingredient inventory
- WorkerTypes          : Staff role types
- Workers              : Catering staff
- WorkerSalaries       : Salary records
- FavoriteCaterers     : Customer favorite caterers
- Messages             : Customer-Caterer messaging

BUSINESS RULES:
- Minimum 7 days advance booking
- Minimum 50 guests per booking
- Cancellation charges vary by caterer (CancellationChargePercent)
*/
