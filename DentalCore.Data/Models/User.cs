using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class User
{
    public int Id { get; set; }
    public UserRole Role { get; set; }
    public bool IsEnabled { get; set; }
    
    [MaxLength(20)]
    public string Login { get; set; }
    
    public string PasswordHash { get; set; }
    
    [MaxLength(20)]
    public string Name { get; set; }
    
    [MaxLength(20)]
    public string Surname { get; set; }
    
    [MaxLength(10)]
    public string Phone { get; set; }
}

public enum UserRole
{
    Admin,
    Doctor,
    Guest
}