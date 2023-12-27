using DentalCore.Wpf.Abstract;

namespace DentalCore.Wpf.ViewModels.Inners;

public class PhoneListItemViewModel : BaseViewModel
{
    private string _phoneNumber;
    private bool _isMain;
    private string? _tag;

    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            if (value == _phoneNumber) return;
            _phoneNumber = value;
            OnPropertyChanged();
        }
    }

    public bool IsMain
    {
        get => _isMain;
        set
        {
            if (value == _isMain) return;
            _isMain = value;
            OnPropertyChanged();
        }
    }

    public string? Tag
    {
        get => _tag;
        set
        {
            if (value == _tag) return;
            _tag = value;
            OnPropertyChanged();
        }
    }
}