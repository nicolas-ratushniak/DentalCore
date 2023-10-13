using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class PatientInfoViewModel : BaseViewModel
{
    private readonly ObservableCollection<VisitOfPatientListItemViewModel> _visits;
    private readonly IVisitService _visitService;
    private readonly IPaymentService _paymentService;
    private readonly IPatientService _patientService;
    private readonly int _patientId;
    private int _debt;
    private string _name;
    private string _surname;
    private string _patronymic;
    private string _ageString;

    public ICommand AddVisitCommand { get; }
    public ICommand ShowVisitCommand { get; }

    public ICollectionView VisitCollectionView { get; }
    
    public ObservableCollection<string> AllergyNames { get; }
    public ObservableCollection<string> DiseasesNames { get; }

    public int Debt
    {
        get => _debt;
        set
        {
            if (value == _debt) return;
            _debt = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasDebt));
        }
    }

    public string Name
    {
        get => _name;
        private set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }

    public string Surname
    {
        get => _surname;
        private set
        {
            if (value == _surname) return;
            _surname = value;
            OnPropertyChanged();
        }
    }

    public string Patronymic
    {
        get => _patronymic;
        private set
        {
            if (value == _patronymic) return;
            _patronymic = value;
            OnPropertyChanged();
        }
    }

    public string AgeString
    {
        get => _ageString;
        private set
        {
            if (value == _ageString) return;
            _ageString = value;
            OnPropertyChanged();
        }
    }

    public bool HasAllergiesOrDiseases => AllergyNames.Any() || DiseasesNames.Any();
    public bool HasDebt => Debt > 0;

    public PatientInfoViewModel(
        int id,
        INavigationService navigationService,
        IPatientService patientService,
        IVisitService visitService,
        IPaymentService paymentService
    )
    {
        _visitService = visitService;
        _paymentService = paymentService;
        _patientId = id;
        _patientService = patientService;
        
        _visits = new ObservableCollection<VisitOfPatientListItemViewModel>();
        AllergyNames = new ObservableCollection<string>();
        DiseasesNames = new ObservableCollection<string>();

        VisitCollectionView = CollectionViewSource.GetDefaultView(_visits);
        
        VisitCollectionView.SortDescriptions.Add(
            new SortDescription(nameof(VisitOfPatientListItemViewModel.Date), ListSortDirection.Descending));

        AddVisitCommand = new RelayCommand<object>(_ =>
            navigationService.NavigateTo(ViewType.VisitCreate, id));

        ShowVisitCommand = new RelayCommand<int>(visitId =>
            navigationService.NavigateTo(ViewType.VisitInfo, visitId));

        LoadedCommand = new AsyncRelayCommand(LoadData);
    }

    private async Task LoadData()
    {
        var patient = await _patientService.GetAsync(_patientId);

        Name = patient.Name;
        Surname = patient.Surname;
        Patronymic = patient.Patronymic;
        Debt = await _paymentService.GetPatientDebtAsync(_patientId);

        var age = CalculateAge(patient.BirthDate);
        AgeString = age == 1 ? "1 рік" : $"{age} років";

        foreach (var allergy in await _patientService.GetAllergiesAsync(_patientId))
        {
            AllergyNames.Add(allergy.Name);
        }

        foreach (var disease in await _patientService.GetDiseasesAsync(_patientId))
        {
            DiseasesNames.Add(disease.Name);
        }
        OnPropertyChanged(nameof(HasAllergiesOrDiseases));

        foreach (var visit in await GetVisitsAsync())
        {
            _visits.Add(visit);
        }
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;

        var a = (today.Year * 100 + today.Month) * 100 + today.Day;
        var b = (birthDate.Year * 100 + birthDate.Month) * 100 + birthDate.Day;

        return (a - b) / 10000;
    }

    private async Task<IEnumerable<VisitOfPatientListItemViewModel>> GetVisitsAsync()
    {
        return (await _visitService.GetAllAsync())
            .Where(v => v.PatientId == _patientId)
            .Select(v => new VisitOfPatientListItemViewModel
            {
                Id = v.Id,
                Date = v.CreatedOn,
                Diagnosis = v.Diagnosis
            });
    }
}