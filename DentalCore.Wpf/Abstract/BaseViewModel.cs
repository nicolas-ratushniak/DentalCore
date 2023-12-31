using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DentalCore.Wpf.Commands;

namespace DentalCore.Wpf.Abstract;

public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand LoadedCommand { get; protected init; }

    protected BaseViewModel()
    {
        LoadedCommand = new AsyncRelayCommand(LoadDataAsync, ex => 
            MessageBox.Show(
                $"Зверніться по допомогу до розробника \n{ex.Message}", 
                "Упс, помилка", 
                MessageBoxButton.OK, 
                MessageBoxImage.Error));
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual Task LoadDataAsync()
    {
        return Task.CompletedTask;
    }
    
    public virtual void Dispose()
    {
    }
}