namespace OnlineCatering.Models;

public class WorkerType
{
    public int WorkerTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PayPerDay { get; set; }

    public ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
