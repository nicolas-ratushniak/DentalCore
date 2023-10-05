using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalCore.Data.Models;

public class Patient
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public Gender Gender { get; set; }
    [MaxLength(30)] public string Name { get; set; }
    [MaxLength(30)] public string Surname { get; set; }
    [MaxLength(30)] public string Patronymic { get; set; }
    [MaxLength(10)] public string Phone { get; set; }
    [Column(TypeName = "date")] public DateTime BirthDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedOn { get; set; }
    public DateTime CreatedOn { get; set; }

    public List<Allergy> Allergies { get; set; } = new();
    public List<PatientAllergy> PatientAllergies { get; set; } = new();
    public List<Disease> Diseases { get; set; } = new();
    public List<PatientDisease> PatientDiseases { get; set; } = new();
    public List<Visit> Visits { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
}

public enum Gender
{
    Male = 1,
    Female = 2
}