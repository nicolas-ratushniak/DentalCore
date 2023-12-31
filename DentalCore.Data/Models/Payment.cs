﻿namespace DentalCore.Data.Models;

public class Payment
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public Visit Visit { get; set; }
    public int Sum { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedOn { get; set; }
}