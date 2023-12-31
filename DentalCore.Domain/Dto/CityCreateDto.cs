using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class CityCreateDto
{
    [Required(ErrorMessage = "Назва міста обов'язкова")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Назва міста повинна мати від 2 до 30 символів")]
    public string Name { get; set; }
}