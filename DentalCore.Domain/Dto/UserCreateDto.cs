using System.ComponentModel.DataAnnotations;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Dto;

public class UserCreateDto
{
    [Required]
    public UserRole Role { get; set; }
    
    [Required]
    [RegularExpression(@"^[A-Za-z]\w{0,19}$")]
    public string Login { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Surname { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression(@"^0\d{9}$")]
    public string Phone { get; set; } = string.Empty;
}