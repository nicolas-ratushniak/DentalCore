using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class TreatmentItemDto
{
    [Required]
    public int ProcedureId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}