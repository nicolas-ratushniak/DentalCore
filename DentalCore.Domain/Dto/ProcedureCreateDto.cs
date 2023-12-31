using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class ProcedureCreateDto
{
    [Required(ErrorMessage = "Поле Назва обов'язкове")]
    [StringLength(70, MinimumLength = 2, ErrorMessage = "Назва повинна бути від 2 до 30 символів")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Поле Ціна обов'язкове")]
    [Range(1, 100_000, ErrorMessage = "Ціна повинна бути від 1 до 100 000 грн")]
    public int Price { get; set; }
    
    [Required]
    public bool IsDiscountAllowed { get; set; }
}