using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Allergy
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    
    [MaxLength(40)]
    public string Name { get; set; }
}