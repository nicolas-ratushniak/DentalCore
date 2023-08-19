namespace DentalCore.Domain.Models;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public Gender Type { get; set; }
    public DateOnly BirthDate { get; set; }
    public int CityId { get; set; }
    public string PhoneNumber { get; set; }
    public int Debt { get; set; }
    
    public City City { get; set; }
    public ICollection<Allergy>? Allergies { get; set; }
}

public enum Gender
{
    Male,
    Female
}