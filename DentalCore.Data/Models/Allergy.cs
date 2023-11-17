using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Allergy
{
    public int Id { get; set; }
    [MaxLength(50)] public string Name { get; set; }

    public List<Patient> Patients { get; set; } = new();
    public List<PatientAllergy> PatientAllergies { get; set; } = new();
}