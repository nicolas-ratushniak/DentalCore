using System;

namespace DentalCore.Wpf.ViewModels.Inners;

public class TodayVisitListItemViewModel
{
    public int Id { get; set; }
    public TimeOnly Time { get; set; }
    public string? Diagnosis { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }

    public string ShortName => $"{Surname} {Name[0]}.{Patronymic[0]}.";
}