using DentalCore.Data.Models;

namespace DentalCore.Domain.Dto;

public class UserDto
{
    public int Id { get; set; }
    public UserRole Role { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
}