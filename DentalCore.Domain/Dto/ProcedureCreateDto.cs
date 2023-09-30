using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class ProcedureCreateDto
{
    [Required]
    [StringLength(45, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Price { get; set; }
    
    [Required]
    public bool IsDiscountAllowed { get; set; }
}