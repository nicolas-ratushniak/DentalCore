namespace DentalCore.Domain.Dto;

public class PaymentDto
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public int Sum { get; set; }
    public DateTime PaymentDate { get; set; }
}