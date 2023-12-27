using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Views.Pages;

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