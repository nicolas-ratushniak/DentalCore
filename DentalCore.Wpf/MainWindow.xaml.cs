using System.Windows;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.ViewModels;

namespace DentalCore.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(BaseViewModel contextViewModel)
    {
        InitializeComponent();
        DataContext = contextViewModel;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var viewModel = (BaseViewModel)DataContext;
        viewModel.LoadedCommand.Execute(null);
    }
}