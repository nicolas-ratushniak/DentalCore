namespace DentalCore.Domain.Models;

public class Visit
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int PriceNoDiscount { get; set; }
    public int DiscountInUan { get; set; }

    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public ICollection<TreatmentItem>? TreatmentItems { get; set; }
}