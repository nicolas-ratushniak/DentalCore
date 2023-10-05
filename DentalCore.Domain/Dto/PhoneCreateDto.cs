using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class PhoneCreateDto
{
    [Required]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Телефон має починатися з 0 і містити 10 цифр")]
    public string PhoneNumber { get; set; }

    [Required] public bool IsMain { get; set; }
    public string? Tag { get; set; }
}