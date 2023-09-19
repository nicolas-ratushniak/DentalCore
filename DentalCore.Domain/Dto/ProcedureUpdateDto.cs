using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class ProcedureUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(30, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Price { get; set; }
    
    [Required]
    public bool IsDiscountValid { get; set; }
}