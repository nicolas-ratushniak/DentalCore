using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class VisitsViewModel : BaseViewModel
{
    private readonly IVisitService _visitService;
    private readonly IPatientService _patientService;
    private string _visitSearchFilter = string.Empty;
    private readonly ObservableCollection<TodayVisitListItemViewModel> _todayVisits;

    public ICommand GoToVisitInfoCommand { get; }

    public ICollectionView VisitCollectionView { get; }

    public string VisitSearchFilter
    {
        get => _visitSearchFilter;
        set
        {
            if (value == _visitSearchFilter) return;
            _visitSearchFilter = value;

            OnPropertyChanged();
            VisitCollectionView.Refresh();
        }
    }

    public VisitsViewModel(
        INavigationService navigationService,
        IVisitService visitService,
        IPatientService patientService)
    {
        _visitService = visitService;
        _patientService = patientService;

        _todayVisits = new ObservableCollection<TodayVisitListItemViewModel>();

        VisitCollectionView = CollectionViewSource.GetDefaultView(_todayVisits);
        
        VisitCollectionView.SortDescriptions.Add(
            new SortDescription(nameof(TodayVisitListItemViewModel.Time), ListSortDirection.Descending));

        VisitCollectionView.Filter = o =>
        {
            if (o is TodayVisitListItemViewModel tv)
            {
                return tv.Surname.ToLower().StartsWith(VisitSearchFilter.ToLower()) ||
                       tv.Name.ToLower().StartsWith(VisitSearchFilter.ToLower());
            }

            return false;
        };

        GoToVisitInfoCommand = new RelayCommand<int>(id => navigationService.NavigateTo(ViewType.VisitInfo, id));
        LoadedCommand = new AsyncRelayCommand(LoadData);
    }

    private async Task LoadData()
    {
        foreach (var visit in await GetVisitsForTodayAsync())
        {
            _todayVisits.Add(visit);
        }
    }

    private async Task<List<TodayVisitListItemViewModel>> GetVisitsForTodayAsync()
    {
        var patients = (await _patientService.GetAllAsync()).ToList();

        return (await _visitService.GetAllAsync())
            .Where(v => v.CreatedOn.Date == DateTime.Today)
            .Select(v =>
            {
                var patient = patients.Single(p => p.Id == v.PatientId);

                return new TodayVisitListItemViewModel
                {
                    Id = v.Id,
                    Time = TimeOnly.FromDateTime(v.CreatedOn),
                    Diagnosis = v.Diagnosis,
                    Name = patient.Name,
                    Surname = patient.Surname,
                    Patronymic = patient.Patronymic
                };
            })
            .ToList();
    }
}