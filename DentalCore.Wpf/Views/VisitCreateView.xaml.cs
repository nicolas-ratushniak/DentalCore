using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.ViewModels;

namespace DentalCore.Wpf.Views;

public partial class VisitCreateView : UserControl
{
    public VisitCreateView()
    {
        InitializeComponent();
    }

    private void TreatmentItem_OnQuantityChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
        var viewModel = (VisitCreateViewModel)DataContext;
        viewModel.UpdatePriceCommand.Execute(null);
    }
}