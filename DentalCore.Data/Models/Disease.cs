using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Disease
{
    public int Id { get; set; }
    
    [MaxLength(20)]
    public string Name { get; set; }

    public ICollection<Patient>? Patients { get; set; }
}