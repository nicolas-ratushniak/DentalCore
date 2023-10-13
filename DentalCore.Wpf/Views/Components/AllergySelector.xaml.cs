using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DentalCore.Wpf.ViewModels;
using DentalCore.Wpf.ViewModels.Components;

namespace DentalCore.Wpf.Views.Components;

public partial class AllergySelector : UserControl
{
    public AllergySelector()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}