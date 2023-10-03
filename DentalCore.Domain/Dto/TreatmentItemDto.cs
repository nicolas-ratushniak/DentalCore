using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class TreatmentItemDto
{
    [Required]
    public int ProcedureId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Кількість у процедурі має бути більше 1")]
    public int Quantity { get; set; }
}