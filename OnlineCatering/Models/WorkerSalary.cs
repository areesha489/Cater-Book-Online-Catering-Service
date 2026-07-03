namespace OnlineCatering.Models;

public class WorkerSalary
{
    public int SalaryId { get; set; }
    public int WorkerId { get; set; }
    public int WorkingDays { get; set; }
    public int PayMonth { get; set; }
    public int PayYear { get; set; }
    public decimal TotalPay { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Paid";

    public Worker? Worker { get; set; }
}
