namespace DentalCore.Domain.Dto;

public class PhoneDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsMain { get; set; }
    public string? Tag { get; set; }
}