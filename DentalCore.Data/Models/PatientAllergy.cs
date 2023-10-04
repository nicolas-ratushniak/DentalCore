namespace DentalCore.Data.Models;

public class PatientAllergy
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    public int AllergyId { get; set; }
    public Allergy Allergy { get; set; }
    public DateTime DateAdded { get; set; }
}