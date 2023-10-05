using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Visit
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    public int DoctorId { get; set; }
    public User Doctor { get; set; }
    public int DiscountSum { get; set; }
    public int TotalPrice { get; set; }
    [MaxLength(100)] public string? Diagnosis { get; set; }
    public DateTime CreatedOn { get; set; }

    public List<TreatmentItem> TreatmentItems { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}