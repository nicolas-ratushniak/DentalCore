using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Disease
{
    public int Id { get; set; }
    [MaxLength(40)] public string Name { get; set; }

    public List<Patient> Patients { get; set; } = new();
    public List<PatientDisease> PatientDiseases { get; set; } = new();
}