using System.ComponentModel.DataAnnotations;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Dto;

public class PatientCreateDto
{
    [Required]
    public int CityId { get; set; }
    
    [Required]
    public Gender Gender { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Patronymic { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^0\d{9}$")]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    public DateTime BirthDate { get; set; }
    
    public IEnumerable<string>? AllergyNames { get; set; }
    
    public IEnumerable<int>? DiseaseIds { get; set; }
}