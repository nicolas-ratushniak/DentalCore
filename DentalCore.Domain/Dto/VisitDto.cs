namespace DentalCore.Domain.Dto;

public class VisitDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime VisitDate { get; set; }
    public int TotalPrice { get; set; }
    public string? Diagnosis { get; set; }
}