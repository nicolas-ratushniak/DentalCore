namespace DentalCore.Data.Models;

public class PatientDisease
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    public int DiseaseId { get; set; }
    public Disease Disease { get; set; }
    public DateTime DateAdded { get; set; }
}