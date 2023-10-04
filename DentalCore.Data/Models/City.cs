using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class City
{
    public int Id { get; set; }
    [MaxLength(30)] public string Name { get; set; }
}