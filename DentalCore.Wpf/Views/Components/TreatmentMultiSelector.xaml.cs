using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.ViewModels.Components;

namespace DentalCore.Wpf.Views.Components;

public partial class TreatmentMultiSelector : UserControl
{
    public TreatmentMultiSelector()
    {
        InitializeComponent();
    }
    
    private void TreatmentItem_OnQuantityChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
        var viewModel = (TreatmentMultiSelectorViewModel)DataContext;
        viewModel.UpdatePriceCommand.Execute(null);
    }
}