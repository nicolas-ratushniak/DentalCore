using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.ViewModels;

namespace DentalCore.Wpf.Views;

public partial class PatientsView : UserControl
{
    public PatientsView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}