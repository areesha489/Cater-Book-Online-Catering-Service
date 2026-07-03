using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Models;
using OnlineCatering.Services;

namespace OnlineCatering.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Caterers.AnyAsync()) return;

        var caterer1 = new Caterer
        {
            BusinessName = "Royal Feast Caterers",
            OwnerName = "Ahmed Khan",
            Email = "info@royalfeast.com",
            Phone = "03001234567",
            Address = "Main Boulevard, Gulberg",
            City = "Lahore",
            Area = "Gulberg",
            FoodType = "Both",
            MinGuests = 50,
            MaxGuests = 2000,
            CancellationChargePercent = 15,
            Description = "Premium catering for weddings, corporate events, and private parties.",
            Rating = 4.8m,
            ImageUrl = CatererImageHelper.RoyalFeastImageUrl
        };

        var caterer2 = new Caterer
        {
            BusinessName = "Spice Garden Catering",
            OwnerName = "Fatima Ali",
            Email = "contact@spicegarden.com",
            Phone = "03009876543",
            Address = "Clifton Block 5",
            City = "Karachi",
            Area = "Clifton",
            FoodType = "Veg",
            MinGuests = 50,
            MaxGuests = 1500,
            CancellationChargePercent = 10,
            Description = "Authentic vegetarian cuisine for all occasions.",
            Rating = 4.5m,
            ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=600&q=80"
        };

        var caterer3 = new Caterer
        {
            BusinessName = "Grand Events Catering",
            OwnerName = "Usman Malik",
            Email = "book@grandevents.com",
            Phone = "03211234567",
            Address = "F-7 Markaz",
            City = "Islamabad",
            Area = "F-7",
            FoodType = "Non-Veg",
            MinGuests = 50,
            MaxGuests = 3000,
            CancellationChargePercent = 20,
            Description = "Luxury non-veg catering with live cooking stations.",
            Rating = 4.7m,
            ImageUrl = "https://images.unsplash.com/photo-1467003909585-2f8a72700288?w=600&q=80"
        };

        var caterer4 = new Caterer
        {
            BusinessName = "Al-Rehman Biryani House",
            OwnerName = "Bilal Hussain",
            Email = "info@alrehman.com",
            Phone = "03331234567",
            Address = "Johar Town Block H",
            City = "Lahore",
            Area = "Johar Town",
            FoodType = "Non-Veg",
            MinGuests = 50,
            MaxGuests = 800,
            CancellationChargePercent = 12,
            Description = "Famous biryani and karahi specialists for weddings and mehndi events.",
            Rating = 4.6m,
            ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=600&q=80"
        };

        var caterer5 = new Caterer
        {
            BusinessName = "Green Leaf Veg Catering",
            OwnerName = "Sana Tariq",
            Email = "hello@greenleaf.com",
            Phone = "03451234567",
            Address = "North Nazimabad Block B",
            City = "Karachi",
            Area = "North Nazimabad",
            FoodType = "Veg",
            MinGuests = 50,
            MaxGuests = 1200,
            CancellationChargePercent = 8,
            Description = "Pure vegetarian catering — ideal for corporate and family gatherings.",
            Rating = 4.4m,
            ImageUrl = "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=600&q=80"
        };

        var caterer6 = new Caterer
        {
            BusinessName = "Saffron Delights",
            OwnerName = "Hamza Sheikh",
            Email = "book@saffron.com",
            Phone = "03111234568",
            Address = "Bahria Town Phase 4",
            City = "Rawalpindi",
            Area = "Bahria Town",
            FoodType = "Both",
            MinGuests = 100,
            MaxGuests = 2500,
            CancellationChargePercent = 15,
            Description = "Premium buffet and BBQ catering for large-scale events.",
            Rating = 4.9m,
            ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=600&q=80"
        };

        var caterer7 = new Caterer
        {
            BusinessName = "Desi Feast Caterers",
            OwnerName = "Imran Qureshi",
            Email = "contact@desifeast.com",
            Phone = "03051234569",
            Address = "D Ground Commercial",
            City = "Faisalabad",
            Area = "D Ground",
            FoodType = "Both",
            MinGuests = 50,
            MaxGuests = 1500,
            CancellationChargePercent = 10,
            Description = "Traditional Pakistani cuisine — daal, BBQ, desserts and full wedding packages.",
            Rating = 4.3m,
            ImageUrl = "https://images.unsplash.com/photo-1559339352-11d035aa65de?w=600&q=80"
        };

        db.Caterers.AddRange(caterer1, caterer2, caterer3, caterer4, caterer5, caterer6, caterer7);
        await db.SaveChangesAsync();

        db.LoginMaster.AddRange(
            new LoginMaster { Username = "caterer1", Password = "123456", UserType = "Caterer", ReferenceId = caterer1.CatererId },
            new LoginMaster { Username = "caterer2", Password = "123456", UserType = "Caterer", ReferenceId = caterer2.CatererId },
            new LoginMaster { Username = "caterer3", Password = "123456", UserType = "Caterer", ReferenceId = caterer3.CatererId },
            new LoginMaster { Username = "caterer4", Password = "123456", UserType = "Caterer", ReferenceId = caterer4.CatererId },
            new LoginMaster { Username = "caterer5", Password = "123456", UserType = "Caterer", ReferenceId = caterer5.CatererId }
        );

        var customer = new Customer
        {
            FullName = "Demo Customer",
            Email = "customer@demo.com",
            Phone = "03111234567",
            Address = "Model Town",
            City = "Lahore"
        };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();

        db.LoginMaster.Add(new LoginMaster
        {
            Username = "customer1",
            Password = "123456",
            UserType = "Customer",
            ReferenceId = customer.CustomerId
        });

        // Menu for caterer1
        var cat1Categories = new[]
        {
            new MenuCategory { CatererId = caterer1.CatererId, CategoryName = "Main Course", Description = "Traditional main dishes" },
            new MenuCategory { CatererId = caterer1.CatererId, CategoryName = "Desserts", Description = "Sweet treats" },
            new MenuCategory { CatererId = caterer1.CatererId, CategoryName = "Beverages", Description = "Drinks and refreshments" }
        };
        db.MenuCategories.AddRange(cat1Categories);
        await db.SaveChangesAsync();

        db.Menus.AddRange(
            new Menu { CategoryId = cat1Categories[0].CategoryId, ItemName = "Chicken Biryani", Description = "Aromatic basmati rice with tender chicken", PricePerPlate = 450, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Chicken Biryani") },
            new Menu { CategoryId = cat1Categories[0].CategoryId, ItemName = "Mutton Karahi", Description = "Spicy mutton karahi", PricePerPlate = 650, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Mutton Karahi") },
            new Menu { CategoryId = cat1Categories[0].CategoryId, ItemName = "Seekh Kebab", Description = "Juicy minced meat skewers, charcoal grilled", PricePerPlate = 380, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Seekh Kebab") },
            new Menu { CategoryId = cat1Categories[0].CategoryId, ItemName = "Vegetable Pulao", Description = "Fragrant vegetable rice", PricePerPlate = 350, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Vegetable Pulao") },
            new Menu { CategoryId = cat1Categories[1].CategoryId, ItemName = "Gulab Jamun", Description = "Classic sweet", PricePerPlate = 80, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Gulab Jamun") },
            new Menu { CategoryId = cat1Categories[1].CategoryId, ItemName = "Kheer", Description = "Rice pudding", PricePerPlate = 100, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Kheer") },
            new Menu { CategoryId = cat1Categories[2].CategoryId, ItemName = "Fresh Juice", Description = "Seasonal fruit juice", PricePerPlate = 120, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Fresh Juice") }
        );

        // Menu for caterer2
        var cat2Categories = new[]
        {
            new MenuCategory { CatererId = caterer2.CatererId, CategoryName = "Starters", Description = "Appetizers" },
            new MenuCategory { CatererId = caterer2.CatererId, CategoryName = "Main Course", Description = "Vegetarian mains" }
        };
        db.MenuCategories.AddRange(cat2Categories);
        await db.SaveChangesAsync();

        db.Menus.AddRange(
            new Menu { CategoryId = cat2Categories[0].CategoryId, ItemName = "Samosa Chaat", Description = "Crispy samosa with chutneys", PricePerPlate = 150, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Samosa Chaat") },
            new Menu { CategoryId = cat2Categories[0].CategoryId, ItemName = "Paneer Tikka", Description = "Grilled cottage cheese", PricePerPlate = 280, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Paneer Tikka") },
            new Menu { CategoryId = cat2Categories[1].CategoryId, ItemName = "Dal Makhani", Description = "Creamy black lentils", PricePerPlate = 320, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Dal Makhani") },
            new Menu { CategoryId = cat2Categories[1].CategoryId, ItemName = "Paneer Butter Masala", Description = "Rich paneer curry", PricePerPlate = 380, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Paneer Butter Masala") }
        );

        // Menu for caterer3
        var cat3Categories = new[]
        {
            new MenuCategory { CatererId = caterer3.CatererId, CategoryName = "BBQ", Description = "Grilled specialties" },
            new MenuCategory { CatererId = caterer3.CatererId, CategoryName = "Main Course", Description = "Premium mains" }
        };
        db.MenuCategories.AddRange(cat3Categories);
        await db.SaveChangesAsync();

        db.Menus.AddRange(
            new Menu { CategoryId = cat3Categories[0].CategoryId, ItemName = "Seekh Kebab", Description = "Minced meat skewers", PricePerPlate = 350, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Seekh Kebab") },
            new Menu { CategoryId = cat3Categories[0].CategoryId, ItemName = "Chicken Tikka", Description = "Marinated grilled chicken", PricePerPlate = 400, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Chicken Tikka") },
            new Menu { CategoryId = cat3Categories[1].CategoryId, ItemName = "Beef Nihari", Description = "Slow-cooked beef stew", PricePerPlate = 550, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Beef Nihari") },
            new Menu { CategoryId = cat3Categories[1].CategoryId, ItemName = "Fish Fry", Description = "Crispy fried fish", PricePerPlate = 480, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("Fish Fry") }
        );

        // Quick menus for caterer4–7
        foreach (var c in new[] { caterer4, caterer5, caterer6, caterer7 })
        {
            var cat = new MenuCategory { CatererId = c.CatererId, CategoryName = "Main Course", Description = "Popular dishes" };
            db.MenuCategories.Add(cat);
            await db.SaveChangesAsync();
            db.Menus.AddRange(
                new Menu { CategoryId = cat.CategoryId, ItemName = "Special Biryani", Description = "House special", PricePerPlate = 420, IsVeg = c.FoodType == "Veg", ImageUrl = DishImageHelper.GetImageUrl("Special Biryani") },
                new Menu { CategoryId = cat.CategoryId, ItemName = "BBQ Platter", Description = "Mixed grill", PricePerPlate = 550, IsVeg = false, ImageUrl = DishImageHelper.GetImageUrl("BBQ Platter") },
                new Menu { CategoryId = cat.CategoryId, ItemName = "Dessert Box", Description = "Assorted sweets", PricePerPlate = 120, IsVeg = true, ImageUrl = DishImageHelper.GetImageUrl("Dessert Box") }
            );
        }

        db.WorkerTypes.AddRange(
            new WorkerType { TypeName = "Chef", Description = "Head and assistant chefs", PayPerDay = 3500 },
            new WorkerType { TypeName = "Waiter", Description = "Serving staff", PayPerDay = 2000 },
            new WorkerType { TypeName = "Cleaner", Description = "Cleaning staff", PayPerDay = 1500 },
            new WorkerType { TypeName = "Supervisor", Description = "Event supervisors", PayPerDay = 4000 },
            new WorkerType { TypeName = "Helper", Description = "Kitchen helpers", PayPerDay = 1800 }
        );
        await db.SaveChangesAsync();

        if (!await db.LoginMaster.AnyAsync(l => l.UserType == "Admin"))
        {
            db.LoginMaster.Add(new LoginMaster
            {
                Username = "admin",
                Password = "admin123",
                UserType = "Admin",
                ReferenceId = 0
            });
            await db.SaveChangesAsync();
        }

        db.SiteAnnouncements.AddRange(
            new SiteAnnouncement
            {
                Title = "Pay with Easypaisa, JazzCash or Bank Transfer",
                Message = "Secure online payments — choose your preferred method at checkout.",
                IsActive = true
            },
            new SiteAnnouncement
            {
                Title = "Browse All Restaurants — No Name Needed!",
                Message = "Explore our full catering list and book your favorite service easily.",
                IsActive = true
            }
        );
        await db.SaveChangesAsync();
    }

    public static async Task EnsureAdminAsync(ApplicationDbContext db)
    {
        if (!await db.LoginMaster.AnyAsync(l => l.UserType == "Admin"))
        {
            db.LoginMaster.Add(new LoginMaster
            {
                Username = "admin",
                Password = "admin123",
                UserType = "Admin",
                ReferenceId = 0
            });
            await db.SaveChangesAsync();
        }
    }

    public static async Task EnsureCatererImagesAsync(ApplicationDbContext db)
    {
        var caterers = await db.Caterers.OrderBy(c => c.CatererId).ToListAsync();
        var updated = false;

        for (var i = 0; i < caterers.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(caterers[i].ImageUrl) ||
                OnlineCatering.Services.CatererImageHelper.IsPlaceholderPath(caterers[i].ImageUrl))
            {
                caterers[i].ImageUrl = OnlineCatering.Services.CatererImageHelper.GetDefaultForIndex(i);
                updated = true;
            }
        }

        if (updated)
            await db.SaveChangesAsync();
    }

    public static async Task EnsureRoyalFeastImageAsync(ApplicationDbContext db)
    {
        var royalFeast = await db.Caterers
            .FirstOrDefaultAsync(c => c.BusinessName.Contains("Royal Feast"));
        if (royalFeast == null) return;

        if (royalFeast.ImageUrl != CatererImageHelper.RoyalFeastImageUrl)
        {
            royalFeast.ImageUrl = CatererImageHelper.RoyalFeastImageUrl;
            await db.SaveChangesAsync();
        }
    }

    public static async Task EnsureRoyalFeastMenuAsync(ApplicationDbContext db)
    {
        var royalFeast = await db.Caterers
            .Include(c => c.MenuCategories)
            .ThenInclude(mc => mc.MenuItems)
            .FirstOrDefaultAsync(c => c.BusinessName.Contains("Royal Feast"));
        if (royalFeast == null) return;

        var mainCourse = royalFeast.MenuCategories
            .FirstOrDefault(c => c.CategoryName == "Main Course");
        if (mainCourse == null) return;

        var updated = false;

        if (!mainCourse.MenuItems.Any(m => m.ItemName == "Seekh Kebab"))
        {
            db.Menus.Add(new Menu
            {
                CategoryId = mainCourse.CategoryId,
                ItemName = "Seekh Kebab",
                Description = "Juicy minced meat skewers, charcoal grilled",
                PricePerPlate = 380,
                IsVeg = false,
                ImageUrl = DishImageHelper.GetImageUrl("Seekh Kebab")
            });
            updated = true;
        }

        foreach (var item in mainCourse.MenuItems)
        {
            var expected = DishImageHelper.GetImageUrl(item.ItemName);
            if (item.ItemName is "Mutton Karahi" or "Seekh Kebab" && item.ImageUrl != expected)
            {
                item.ImageUrl = expected;
                updated = true;
            }
        }

        if (updated)
            await db.SaveChangesAsync();
    }

    public static async Task EnsureMenuImagesAsync(ApplicationDbContext db)
    {
        var menus = await db.Menus.ToListAsync();
        var updated = false;

        foreach (var menu in menus)
        {
            var expected = DishImageHelper.GetImageUrl(menu.ItemName);
            if (menu.ImageUrl != expected)
            {
                menu.ImageUrl = expected;
                updated = true;
            }
        }

        if (updated)
            await db.SaveChangesAsync();
    }
}
