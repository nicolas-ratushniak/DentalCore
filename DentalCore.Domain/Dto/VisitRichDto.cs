namespace DentalCore.Domain.Dto;

public class VisitRichDto
{
    public int Id { get; set; }
    public DateTime VisitDate { get; set; }
    public int TotalPrice { get; set; }
    public int ActuallyPayed { get; set; }
    public string? Diagnosis { get; set; }
    public PatientDto Patient { get; set; }
    public UserDto Doctor { get; set; }
}