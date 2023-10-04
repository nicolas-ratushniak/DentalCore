namespace DentalCore.Data.Models;

public class Payment
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public Visit Visit { get; set; }
    public DateTime DateTime { get; set; }
    public int Sum { get; set; }
}