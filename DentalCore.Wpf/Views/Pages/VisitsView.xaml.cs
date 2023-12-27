using System.Windows;
using System.Windows.Controls;
using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.Views.Pages;

public partial class VisitsView : UserControl
{
    public VisitsView()
    {
        InitializeComponent();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        
        viewModel.LoadedCommand.Execute(null);
    }
}