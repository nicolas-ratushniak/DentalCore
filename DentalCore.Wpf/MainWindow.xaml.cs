using System.Windows;
using DentalCore.Wpf.ViewModels;

namespace DentalCore.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow(BaseViewModel contextViewModel)
        {
            InitializeComponent();
            DataContext = contextViewModel;
        }
    }
}