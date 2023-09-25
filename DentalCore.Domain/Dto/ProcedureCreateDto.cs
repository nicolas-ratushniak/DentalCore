using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class ProcedureCreateDto
{
    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Price { get; set; }
    
    [Required]
    public bool IsDiscountValid { get; set; }
}