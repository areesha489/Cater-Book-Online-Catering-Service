using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineCatering.Data;
using OnlineCatering.Models;
using OnlineCatering.Services;
using OnlineCatering.ViewModels;

namespace OnlineCatering.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly AuthService _auth;
    private readonly ImageUploadService _imageUpload;

    public AccountController(ApplicationDbContext db, AuthService auth, ImageUploadService imageUpload)
    {
        _db = db;
        _auth = auth;
        _imageUpload = imageUpload;
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var login = await _auth.ValidateLoginAsync(model.Username, model.Password, model.UserType);
        if (login == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username, password, or user type.");
            return View(model);
        }

        HttpContext.Session.SetInt32(SessionKeys.UserId, login.LoginId);
        HttpContext.Session.SetString(SessionKeys.UserType, login.UserType);
        HttpContext.Session.SetInt32(SessionKeys.ReferenceId, login.ReferenceId);
        HttpContext.Session.SetString(SessionKeys.DisplayName, await GetDisplayName(login));

        return login.UserType switch
        {
            "Customer" => RedirectToAction("Dashboard", "Customer"),
            "Caterer" => RedirectToAction("Dashboard", "Caterer"),
            "Admin" => RedirectToAction("Dashboard", "Admin"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    private async Task<string> GetDisplayName(LoginMaster login) => login.UserType switch
    {
        "Customer" => (await _db.Customers.FindAsync(login.ReferenceId))?.FullName ?? "Customer",
        "Caterer" => (await _db.Caterers.FindAsync(login.ReferenceId))?.BusinessName ?? "Caterer",
        "Admin" => "Administrator",
        _ => login.UserType
    };

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var login = await _db.LoginMaster.FirstOrDefaultAsync(l =>
            l.Username == model.Username && l.UserType == model.UserType && l.IsActive);
        if (login == null)
        {
            ModelState.AddModelError(string.Empty, "User not found.");
            return View(model);
        }

        login.Password = model.NewPassword;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Password reset successful. Please login.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResetUsername() => View(new ResetUsernameViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetUsername(ResetUsernameViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _auth.UsernameExistsAsync(model.NewUsername))
        {
            ModelState.AddModelError(nameof(model.NewUsername), "Username already taken.");
            return View(model);
        }

        var login = await _db.LoginMaster.FirstOrDefaultAsync(l =>
            l.Username == model.CurrentUsername && l.Password == model.Password && l.UserType == "Customer");
        if (login == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(model);
        }

        login.Username = model.NewUsername;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Username updated. Please login with new username.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult RegisterCustomer()
    {
        ViewBag.Cities = AreaData.AllCities;
        return View(new CustomerRegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCustomer(CustomerRegisterViewModel model)
    {
        ViewBag.Cities = AreaData.AllCities;
        if (!ModelState.IsValid) return View(model);
        if (await _auth.UsernameExistsAsync(model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");
            return View(model);
        }

        var customer = new Customer
        {
            FullName = model.FullName,
            Email = model.Email,
            Phone = model.Phone,
            Mobile = model.Phone,
            Address = model.Address,
            City = model.City
        };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        _db.LoginMaster.Add(new LoginMaster
        {
            Username = model.Username,
            Password = model.Password,
            UserType = "Customer",
            ReferenceId = customer.CustomerId
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Registration successful! Please login.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult RegisterCaterer()
    {
        ViewBag.Cities = AreaData.AllCities;
        ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(AreaData.AreasByCity);
        return View(new CatererRegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterCaterer(CatererRegisterViewModel model)
    {
        ViewBag.Cities = AreaData.AllCities;
        ViewBag.AreasJson = System.Text.Json.JsonSerializer.Serialize(AreaData.AreasByCity);
        if (!ModelState.IsValid) return View(model);
        if (await _auth.UsernameExistsAsync(model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");
            return View(model);
        }

        var caterer = new Caterer
        {
            BusinessName = model.BusinessName,
            OwnerName = model.OwnerName,
            Email = model.Email,
            Phone = model.Phone,
            Address = model.Address,
            City = model.City,
            Area = model.Area,
            FoodType = model.FoodType,
            CancellationChargePercent = model.CancellationChargePercent,
            Description = model.Description
        };
        _db.Caterers.Add(caterer);
        await _db.SaveChangesAsync();

        if (model.RestaurantImage != null && model.RestaurantImage.Length > 0)
        {
            var upload = await _imageUpload.SaveCatererImageAsync(model.RestaurantImage, caterer.CatererId);
            if (upload.Success)
            {
                caterer.ImageUrl = upload.RelativePath;
                await _db.SaveChangesAsync();
            }
        }

        _db.LoginMaster.Add(new LoginMaster
        {
            Username = model.Username,
            Password = model.Password,
            UserType = "Caterer",
            ReferenceId = caterer.CatererId
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Caterer registration successful! Please login.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
