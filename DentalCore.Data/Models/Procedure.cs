using System.ComponentModel.DataAnnotations;

namespace DentalCore.Data.Models;

public class Procedure
{
    public int Id { get; set; }
    [MaxLength(45)] public string Name { get; set; }
    public int Price { get; set; }
    public bool IsDiscountAllowed { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DateDeleted { get; set; }
    public DateTime DateUpdated { get; set; }
}