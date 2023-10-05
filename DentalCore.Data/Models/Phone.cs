using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Phone
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    [MaxLength(10)] public string PhoneNumber { get; set; }
    public bool IsMain { get; set; }
    [MaxLength(20)] public string? Tag { get; set; }
}