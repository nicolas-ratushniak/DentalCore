using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class UserUpdateDto
{
    [Required]
    public int Id { get; set; }

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