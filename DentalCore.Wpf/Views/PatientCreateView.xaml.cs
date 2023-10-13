﻿using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.ViewModels;

namespace DentalCore.Wpf.Views;

public partial class PatientCreateView : UserControl
{
    public PatientCreateView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}