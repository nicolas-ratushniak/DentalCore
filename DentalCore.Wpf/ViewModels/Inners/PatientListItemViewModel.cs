namespace DentalCore.Wpf.ViewModels.Inners;

public class PatientListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public string FullName => $"{Surname} {Name} {Patronymic}";
}