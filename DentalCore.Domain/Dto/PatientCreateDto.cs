﻿using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Dto;

public class PatientCreateDto
{
    [Required]
    public int CityId { get; set; }
    
    [Required]
    public bool IsMale { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 2)]
    public string Patronymic { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^0\d{9}$")]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    public DateTime BirthDate { get; set; }
}