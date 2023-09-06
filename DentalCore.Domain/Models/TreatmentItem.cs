namespace DentalCore.Domain.Models;

public class TreatmentItem
{
    public int VisitId { get; set; }
    public int ProcedureId { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public bool IsDiscountValid { get; set; }

    public Visit Visit { get; set; }
    public Procedure Procedure { get; set; }
}