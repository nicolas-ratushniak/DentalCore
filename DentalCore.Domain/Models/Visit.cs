using System.ComponentModel.DataAnnotations.Schema;

namespace DentalCore.Domain.Models;

public class Visit
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int Discount { get; set; }
    public int TotalPrice { get; set; }
    
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    public Patient Patient { get; set; }
    public User Doctor { get; set; }
    public ICollection<TreatmentItem>? TreatmentItems { get; set; }
    public ICollection<Payment> Payments { get; set; }
}