using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.ViewModels;
using DentalCore.Wpf.ViewModels.Components;

namespace DentalCore.Wpf.Views.Components;

public partial class TreatmentSelector : UserControl
{
    public TreatmentSelector()
    {
        InitializeComponent();
    }
    
    private void TreatmentItem_OnQuantityChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
        var viewModel = (TreatmentSelectorComponent)DataContext;
        viewModel.UpdatePriceCommand.Execute(null);
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}