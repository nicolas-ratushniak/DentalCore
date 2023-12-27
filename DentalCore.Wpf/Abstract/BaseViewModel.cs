using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DentalCore.Wpf.Abstract;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand LoadedCommand { get; protected init; }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public virtual void Dispose()
    {
    }
}