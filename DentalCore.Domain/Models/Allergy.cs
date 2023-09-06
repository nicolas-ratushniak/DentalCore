﻿using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Models;

public class Allergy
{
    public int PatientId { get; set; }
    
    [MaxLength(30)]
    public string Name { get; set; }

    public Patient Patient { get; set; }
}