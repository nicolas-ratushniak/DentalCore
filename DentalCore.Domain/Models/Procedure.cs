using System.ComponentModel.DataAnnotations;

namespace DentalCore.Domain.Models;

public class Procedure
{
    public int Id { get; set; }
    
    [MaxLength(30)]
    public string Name { get; set; }
    
    public int Price { get; set; }
    
    public bool IsDiscountValid { get; set; }
}