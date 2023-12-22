namespace DentalCore.Domain.Dto;

public class PaymentCreateDto
{
    public int VisitId { get; set; }
    public int Sum { get; set; }
    public DateTime PaymentDate { get; set; }
}