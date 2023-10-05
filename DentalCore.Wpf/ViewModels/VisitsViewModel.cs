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

public class VisitsViewModel : BaseViewModel
{
    private readonly IVisitService _visitService;
    private readonly IPatientService _patientService;
    private string _visitSearchFilter = string.Empty;

    public ICommand ShowVisitCommand { get; }

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

        VisitCollectionView = CollectionViewSource.GetDefaultView(GetVisitsForToday());
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

        ShowVisitCommand = new RelayCommand<int>(id => navigationService.NavigateTo(ViewType.VisitInfo, id));
    }

    private List<TodayVisitListItemViewModel> GetVisitsForToday()
    {
        var patients = _patientService.GetAll().ToList();

        return _visitService.GetAll()
            .Where(v => v.CreatedOn.Date == DateTime.Today)
            .Select(v =>
            {
                var patient = patients
                    .Single(p => p.Id == v.PatientId);

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