using DentalCore.Data.Models;

namespace DentalCore.Domain.Dto;

public class PatientRichDto
{
    public int Id { get; set; }
    public Gender Gender { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public DateTime BirthDate { get; set; }
    public CityDto City { get; set; }
}