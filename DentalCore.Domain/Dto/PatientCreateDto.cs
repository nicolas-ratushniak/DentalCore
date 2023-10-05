using System.ComponentModel.DataAnnotations;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Dto;

public class PatientCreateDto
{
    [Required] public int CityId { get; set; }
    [Required] public Gender Gender { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Ім'я повинно бути від 2 до 30 символів")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Пізвище повинно бути від 2 до 30 символів")]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "По батькові повинно бути від 2 до 30 символів")]
    public string Patronymic { get; set; } = string.Empty;
    
    [Required] public DateTime BirthDate { get; set; }
    public List<int> AllergyIds { get; set; } = new();
    public List<int> DiseaseIds { get; set; } = new();
    public List<PhoneCreateDto> Phones { get; set; } = new();
}