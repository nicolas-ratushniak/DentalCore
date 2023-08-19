namespace DentalCore.Domain.Models;

public class TreatmentItem
{
    public int VisitId { get; set; }
    public int ProcedureId { get; set; }
    public int Quantity { get; set; }
}