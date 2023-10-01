using System;

namespace DentalCore.Wpf.ViewModels.Inners;

public class VisitOfPatientListItemViewModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Diagnosis { get; set; }

    public string DateString => Date.ToString("dd.MM.yy");
}