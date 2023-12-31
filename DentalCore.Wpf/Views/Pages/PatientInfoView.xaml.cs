using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Views.Pages;

public partial class PatientInfoView : UserControl
{
    public PatientInfoView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is BaseViewModel viewModel)
        {
            viewModel.LoadedCommand.Execute(null);
        }
    }
}