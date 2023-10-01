namespace DentalCore.Wpf.ViewModels.Inners;

public class TreatmentItemListItemViewModel : BaseViewModel
{
    public int Id { get; set; }
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
}