using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalCore.Domain.Models;

public class Patient
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public Gender Gender { get; set; }
    
    [MaxLength(20)]
    public string Name { get; set; }

    [MaxLength(20)]
    public string Surname { get; set; }

    [MaxLength(20)]
    public string Patronymic { get; set; }

    [MaxLength(10)]
    public string Phone { get; set; }

    [Column(TypeName = "date")]
    public DateTime BirthDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime DateCreated { get; set; }

    public City City { get; set; }
    public ICollection<Allergy>? Allergies { get; set; }
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<Disease>? Diseases { get; set; }
}

public enum Gender
{
    Male,
    Female
}