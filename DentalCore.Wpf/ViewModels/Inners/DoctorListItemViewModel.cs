namespace DentalCore.Wpf.ViewModels.Inners;

public class DoctorListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string FullName => $"{Surname} {Name}";
}