using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class TreatmentItemCreateDto
{
    [Required]
    public int ProcedureId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}