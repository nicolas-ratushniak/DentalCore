namespace DentalCore.Wpf.ViewModels.Inners;

public class TreatmentItemListItemViewModel : BaseViewModel
{
    private int _quantity;
    private bool _isSelected;
    public int Id { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value == _isSelected) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value == _quantity) return;
            _quantity = value;
            OnPropertyChanged();
        }
    }

    public string Name { get; set; }
    public int Price { get; set; }
}