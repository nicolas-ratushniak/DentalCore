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
    private readonly IPatientService _patientService;
    private string _patientSearchFilter = string.Empty;

    public ICommand AddPatientCommand { get; }
    public ICommand EditPatientCommand { get; }
    public ICommand ShowPatientCommand { get; }

    public ICollectionView PatientCollectionView { get; }

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
        _patientService = patientService;
        List<PatientListItemViewModel> patientsList = GetPatients();

        PatientCollectionView = CollectionViewSource.GetDefaultView(patientsList);
        PatientCollectionView.SortDescriptions.Add(new SortDescription(nameof(PatientListItemViewModel.FullName), ListSortDirection.Descending));
        PatientCollectionView.Filter = o => o is PatientListItemViewModel p &&
                                            (p.Surname.ToLower().StartsWith(PatientSearchFilter.ToLower()) ||
                                             p.Name.ToLower().StartsWith(PatientSearchFilter.ToLower()));

        AddPatientCommand = new RelayCommand<object>(_ =>
            navigationService.NavigateTo(ViewType.PatientCreate, null));

        EditPatientCommand = new RelayCommand<int>(id =>
            navigationService.NavigateTo(ViewType.PatientUpdate, id));

        ShowPatientCommand = new RelayCommand<int>(id =>
            navigationService.NavigateTo(ViewType.PatientInfo, id));
    }

    private List<PatientListItemViewModel> GetPatients()
    {
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