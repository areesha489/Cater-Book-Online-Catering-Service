namespace OnlineCatering.Models;

public class Worker
{
    public int WorkerId { get; set; }
    public int CatererId { get; set; }
    public int WorkerTypeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;

    public Caterer? Caterer { get; set; }
    public WorkerType? WorkerType { get; set; }
    public ICollection<WorkerSalary> Salaries { get; set; } = new List<WorkerSalary>();
}
