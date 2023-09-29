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

public class PatientsViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IPatientService _patientService;
    private readonly List<PatientListItemViewModel> _patientsList;
    private string _patientSearchFilter = string.Empty;

    public ICommand AddPatientCommand { get; set; }
    public ICommand EditPatientCommand { get; set; }
    public ICommand ShowPatientCommand { get; set; }

    public ICollectionView PatientCollectionView { get; set; }

    public string PatientSearchFilter
    {
        get => _patientSearchFilter;
        set
        {
            if (value == _patientSearchFilter) return;
            _patientSearchFilter = value;

            OnPropertyChanged();
            PatientCollectionView.Refresh();
        }
    }

    public PatientsViewModel(INavigationService navigationService, IPatientService patientService)
    {
        _navigationService = navigationService;
        _patientService = patientService;
        _patientsList = GetPatients();

        PatientCollectionView = CollectionViewSource.GetDefaultView(_patientsList);
        PatientCollectionView.SortDescriptions.Add(new SortDescription(nameof(PatientListItemViewModel.FullName),
            ListSortDirection.Descending));
        PatientCollectionView.Filter = o => o is PatientListItemViewModel p &&
                                            (p.Surname.ToLower().StartsWith(PatientSearchFilter.ToLower()) ||
                                             p.Name.ToLower().StartsWith(PatientSearchFilter.ToLower()));

        AddPatientCommand = new RelayCommand<object>(_ =>
            _navigationService.UpdateCurrentViewType(ViewType.PatientCreate, null));

        EditPatientCommand = new RelayCommand<int>(id =>
            _navigationService.UpdateCurrentViewType(ViewType.PatientUpdate, id));

        ShowPatientCommand = new RelayCommand<int>(id =>
            _navigationService.UpdateCurrentViewType(ViewType.PatientInfo, id));
    }

    private List<PatientListItemViewModel> GetPatients()
    {
        // return new List<PatientListItemViewModel>
        // {
        //     new(1, "Сергій", "Іванов", "Олександрович"),
        //     new(2, "Марина", "Іванова", "Олександрівна"),
        //     new(3, "Андрій", "Степаненко", "Ростиславович"),
        //     new(1, "Сергій", "Іванов", "Олександрович"),
        //     new(2, "Марина", "Іванова", "Олександрівна"),
        //     new(3, "Андрій", "Степаненко", "Ростиславович"),
        //     new(1, "Сергій", "Іванов", "Олександрович"),
        //     new(2, "Марина", "Іванова", "Олександрівна"),
        //     new(3, "Андрій", "Степаненко", "Ростиславович"),
        //     new(1, "Сергій", "Іванов", "Олександрович"),
        //     new(2, "Марина", "Іванова", "Олександрівна"),
        //     new(3, "Андрій", "Степаненко", "Ростиславович"),
        //     new(1, "Сергій", "Іванов", "Олександрович"),
        //     new(2, "Марина", "Іванова", "Олександрівна"),
        //     new(3, "Андрій", "Степаненко", "Ростиславович"),
        //     new(1, "Сергій", "Іванов", "Олександрович"),
        //     new(2, "Марина", "Іванова", "Олександрівна"),
        //     new(3, "Андрій", "Степаненко", "Ростиславович")
        // };

        return _patientService.GetAll()
            .Select(p => new PatientListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Surname = p.Surname,
                Patronymic = p.Patronymic
            })
            .ToList();
    }
}