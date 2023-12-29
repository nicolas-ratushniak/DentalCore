using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Views.Modals;

public partial class ProcedureUpdateView : UserControl
{
    public ProcedureUpdateView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        viewModel.LoadedCommand.Execute(null);
    }
}