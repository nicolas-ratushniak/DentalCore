using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class PatientInfoViewModel : BaseViewModel
{
    private readonly IPatientService _patientService;
    private readonly IVisitService _visitService;
    private int _debt;

    public ICommand AddVisitCommand { get; }
    public ICommand ShowVisitCommand { get; }

    public ICollectionView VisitCollectionView { get; }

    public int Debt
    {
        get => _debt;
        set
        {
            if (value == _debt) return;
            _debt = value;
            OnPropertyChanged();
        }
    }

    public string Name { get; }
    public string Surname { get; }
    public string Patronymic { get; }
    public string AgeString { get; }
    public IEnumerable<string> AllergyNames { get; }
    public IEnumerable<string> DiseasesNames { get; }
    public bool HasAllergiesOrDiseases => AllergyNames.Any() || DiseasesNames.Any();
    public bool HasDebt => Debt > 0;

    public PatientInfoViewModel(
        int id, 
        INavigationService navigationService,
        IPatientService patientService,
        IVisitService visitService  
        )
    {
        _patientService = patientService;
        _visitService = visitService;

        var patient = patientService.Get(id);

        Name = patient.Name;
        Surname = patient.Surname;
        Patronymic = patient.Patronymic;
        AllergyNames = patientService.GetAllergies(id).Select(a => a.Name);
        DiseasesNames = patientService.GetDiseases(id).Select(d => d.Name);
        Debt = patientService.GetDebt(id);
        // Debt = 100;
        
        var age = CalculateAge(patient.BirthDate);
        AgeString = age == 1 ? "1 рік" : $"{age} років";

        VisitCollectionView = CollectionViewSource.GetDefaultView(GetVisits());
        VisitCollectionView.SortDescriptions.Add(new SortDescription(nameof(VisitOfPatientListItemViewModel.Date), ListSortDirection.Descending));

        AddVisitCommand = new RelayCommand<object>(_ => navigationService.NavigateTo(ViewType.VisitCreate, id));
        ShowVisitCommand = new RelayCommand<int>(visitId => navigationService.NavigateTo(ViewType.VisitInfo, visitId));
    }

    private IEnumerable<VisitOfPatientListItemViewModel> GetVisits()
    {
        return new List<VisitOfPatientListItemViewModel>()
        {
            new()
            {
                Id = 1,
                Date = DateTime.Today,
                Diagnosis = "p12 f3"
            },
            new()
            {
                Id = 2,
                Date = DateTime.Today.AddDays(-3),
                Diagnosis = "Соняшник цвіте в полі, а птахи співають весняні пісні, надія на краще завжди з нами!"
            },
            new()
            {
                Id = 3,
                Date = DateTime.Today.AddDays(-1),
                Diagnosis = "p12 f3 12"
            },
        };
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;

        var a = (today.Year * 100 + today.Month) * 100 + today.Day;
        var b = (birthDate.Year * 100 + birthDate.Month) * 100 + birthDate.Day;

        return (a - b) / 10000;
    }
}