﻿namespace DentalCore.Wpf.ViewModels.Inners;

public class DiseaseListItemViewModel
{
    public int Id { get; set; }
    public bool IsSelected { get; set; }
    public string Name { get; set; } = string.Empty;
}