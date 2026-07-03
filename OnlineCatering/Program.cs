using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ImageUploadService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (app.Environment.IsDevelopment())
        await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(db);
    await DbSeeder.EnsureAdminAsync(db);
    await DbSeeder.EnsureCatererImagesAsync(db);
    await DbSeeder.EnsureRoyalFeastImageAsync(db);
    await DbSeeder.EnsureRoyalFeastMenuAsync(db);
    await DbSeeder.EnsureMenuImagesAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
