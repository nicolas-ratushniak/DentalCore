using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class CityCreateDto
{
    [Required(ErrorMessage = "Поле Назва є обов'язковим")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Назва повинна мати від 2 до 30 символів")]
    public string Name { get; set; }
}