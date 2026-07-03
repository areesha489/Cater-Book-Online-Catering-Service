using System.ComponentModel.DataAnnotations;

namespace OnlineCatering.ViewModels;

public class LoginViewModel
{
    [Required, Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string UserType { get; set; } = "Customer";
}

public class CustomerRegisterViewModel
{
    [Required, Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Phone Number")]
    [RegularExpression(@"^03\d{9}$", ErrorMessage = "Enter a valid Pakistani mobile number (e.g. 03001234567).")]
    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required, Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class CatererRegisterViewModel
{
    [Required, Display(Name = "Business Name")]
    public string BusinessName { get; set; } = string.Empty;

    [Required, Display(Name = "Owner Name")]
    public string OwnerName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Display(Name = "Phone Number")]
    [RegularExpression(@"^03\d{9}$", ErrorMessage = "Enter a valid Pakistani mobile number (e.g. 03001234567).")]
    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required, Display(Name = "Area / Locality")]
    public string Area { get; set; } = string.Empty;

    [Required, Display(Name = "Food Type")]
    public string FoodType { get; set; } = "Both";

    [Display(Name = "Cancellation Charge (%)")]
    [Range(0, 100)]
    public decimal CancellationChargePercent { get; set; } = 10;

    public string Description { get; set; } = string.Empty;

    [Display(Name = "Restaurant Photo")]
    public IFormFile? RestaurantImage { get; set; }

    [Required, Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class CatererSearchViewModel
{
    public string? City { get; set; }
    public string? Area { get; set; }
    public string? FoodType { get; set; }
    public int? GuestCount { get; set; }
    public string? SearchTerm { get; set; }
}

public class CatererBrowseViewModel
{
    public string? City { get; set; }
    public string? Area { get; set; }
    public string? FoodType { get; set; }
    public string SortBy { get; set; } = "Rating";
}

public class BookingViewModel
{
    public int CatererId { get; set; }
    public string CatererName { get; set; } = string.Empty;

    [Required, Display(Name = "Event Date")]
    [DataType(DataType.Date)]
    public DateTime EventDate { get; set; } = DateTime.Today.AddDays(8);

    [Required, Display(Name = "Event Type")]
    public string EventType { get; set; } = "Wedding";

    [Required, Display(Name = "Event Venue")]
    public string EventVenue { get; set; } = string.Empty;

    [Required, Display(Name = "Number of Guests"), Range(50, 10000)]
    public int GuestCount { get; set; } = 50;

    public string? SpecialInstructions { get; set; }

    public List<MenuSelectionItem> SelectedItems { get; set; } = new();
}

public class MenuSelectionItem
{
    public int MenuId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal PricePerPlate { get; set; }
    public bool IsSelected { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class PaymentViewModel
{
    public int OrderId { get; set; }
    public string BookingId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }

    [Required, Display(Name = "Payment Method")]
    public string PaymentMethod { get; set; } = "Easypaisa";

    [Display(Name = "Easypaisa / JazzCash Mobile Number")]
    public string? MobileNumber { get; set; }

    [Display(Name = "Bank Name")]
    public string? BankName { get; set; }

    [Display(Name = "Account Number / IBAN")]
    public string? AccountNumber { get; set; }

    [Display(Name = "Account Title")]
    public string? AccountTitle { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required, Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required, Display(Name = "User Type")]
    public string UserType { get; set; } = "Customer";

    [Required, DataType(DataType.Password), MinLength(6)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ResetUsernameViewModel
{
    [Required, Display(Name = "Current Username")]
    public string CurrentUsername { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, Display(Name = "New Username")]
    public string NewUsername { get; set; } = string.Empty;
}

public class MessageViewModel
{
    public int ReceiverId { get; set; }
    public string ReceiverType { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
}
