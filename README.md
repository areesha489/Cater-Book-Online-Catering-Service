# Online Catering System (ASP.NET Core MVC)

A complete web-based catering platform built with **ASP.NET Core 8 MVC**, **Entity Framework Core**, and **SQL Server**.

## Features

### Customer
- Register & Login
- Search caterers by city, food type, and guest count
- View caterer details and menus
- Book catering services (auto booking ID & bill)
- Online payment (simulated)
- View, manage, and cancel bookings
- Favorite caterers list
- Send/receive messages

### Caterer
- Register & Login
- Manage profile
- Add/Edit/Delete menu categories and food items
- View and update booking status
- Generate invoices after confirmation
- Manage staff (Workers), raw materials, and suppliers
- Messaging with customers

## Prerequisites

1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)  
   OR SQL Server / SQL Server Express

## How to Run

### Option 1: Visual Studio
1. Open `OnlineCatering.sln` in Visual Studio 2022
2. Press **F5** to run

### Option 2: Command Line
```bash
cd "c:\xampp\htdocs\Catering Service Website\OnlineCatering"
dotnet restore
dotnet run
```

Open browser: **https://localhost:5001**

## Demo Login Accounts

| Role     | Username   | Password |
|----------|------------|----------|
| Customer | customer1  | 123456   |
| Caterer  | caterer1   | 123456   |
| Caterer  | caterer2   | 123456   |
| Caterer  | caterer3   | 123456   |

## Database

The database `OnlineCateringDB` is created automatically on first run with sample caterers and menus.

Connection string in `appsettings.json`:
```
Server=(localdb)\mssqllocaldb;Database=OnlineCateringDB;Trusted_Connection=True;...
```

To use SQL Server Express instead, update the connection string:
```
Server=.\SQLEXPRESS;Database=OnlineCateringDB;Trusted_Connection=True;TrustServerCertificate=True
```

## Project Structure

```
OnlineCatering/
├── Controllers/     # Home, Account, Customer, Caterer
├── Models/          # Database entities
├── ViewModels/      # Form & search models
├── Views/           # Razor UI pages
├── Data/            # DbContext & seed data
├── Services/        # Booking, Auth services
└── wwwroot/         # CSS & static files
```

## Workflow

**Customer → Search → Select Menu → Book → Pay → Caterer Confirms → Invoice → Event Delivered**

## Business Rules

- Booking must be **7 days** before the event
- Minimum **50 guests** per booking
- Cancellation charges set per caterer (% of total)
