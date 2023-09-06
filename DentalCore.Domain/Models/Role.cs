using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Models;

public class Role
{
    public int Id { get; set; }
    
    [MaxLength(30)]
    public string Name { get; set; }
}