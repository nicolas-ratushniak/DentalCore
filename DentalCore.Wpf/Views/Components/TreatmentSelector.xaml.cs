using System.Windows;
using System.Windows.Controls;
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
        var viewModel = (TreatmentSelectorViewModel)DataContext;
        viewModel.UpdatePriceCommand.Execute(null);
    }
}