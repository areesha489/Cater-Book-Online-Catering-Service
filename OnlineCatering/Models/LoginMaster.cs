namespace OnlineCatering.Models;

public class LoginMaster
{
    public int LoginId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty; // Customer, Caterer
    public int ReferenceId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
