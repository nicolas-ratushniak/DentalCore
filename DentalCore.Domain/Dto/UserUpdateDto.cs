using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class UserUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    [Required] 
    public bool IsEnabled { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Login { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 1)]
    public string Password { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Name { get; set; }
    
    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Surname { get; set; }
    
    [Required]
    [StringLength(10, MinimumLength = 10)]
    public string Phone { get; set; }
}