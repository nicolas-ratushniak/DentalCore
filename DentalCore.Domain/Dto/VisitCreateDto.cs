using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class VisitCreateDto
{
    [Required]
    public int PatientId { get; set; }
    
    [Required]
    public int DoctorId { get; set; }
    
    [Required]
    [Range(0, 100)]
    public int DiscountPercent { get; set; }

    [Required] 
    [Range(0, int.MaxValue)]
    public int FirstPayment { get; set; }

    [StringLength(100)] 
    public string? Diagnosis { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required] 
    public IEnumerable<TreatmentItemDto> TreatmentItems { get; set; }
}