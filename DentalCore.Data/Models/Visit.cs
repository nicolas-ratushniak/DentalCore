using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalCore.Data.Models;

public class Visit
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int DiscountPercent { get; set; }
    public int TotalPrice { get; set; }

    [MaxLength(100)]
    public string Diagnosis { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    public Patient Patient { get; set; }
    public User Doctor { get; set; }
    public ICollection<TreatmentItem>? TreatmentItems { get; set; }
    public ICollection<Payment> Payments { get; set; }
}