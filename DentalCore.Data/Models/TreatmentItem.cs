namespace DentalCore.Data.Models;

public class TreatmentItem
{
    public int VisitId { get; set; }
    public Visit Visit { get; set; }
    public int ProcedureId { get; set; }
    public Procedure Procedure { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    public bool IsDiscountAllowed { get; set; }
    public int DiscountSum { get; set; }
}