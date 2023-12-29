using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Abstract;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class VisitsViewModel : BaseViewModel
{
    private readonly IVisitService _visitService;
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
        IVisitService visitService)
    {
        _visitService = visitService;

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

        GoToVisitInfoCommand = new RelayCommand<int>(id => navigationService.NavigateTo(PageType.VisitInfo, id));
    }

    public override async Task LoadData()
    {
        _todayVisits.Clear();
        
        foreach (var visit in await GetVisitsForTodayAsync())
        {
            _todayVisits.Add(visit);
        }
    }

    private async Task<List<TodayVisitListItemViewModel>> GetVisitsForTodayAsync()
    {
        var todayStart = DateTime.Today;
        var todayEnd = DateTime.Today.AddDays(1);
        
        return (await _visitService.GetAllRichAsync(todayStart, todayEnd))
            .Select(v => new TodayVisitListItemViewModel
            {
                Id = v.Id,
                Time = TimeOnly.FromDateTime(v.VisitDate),
                Diagnosis = v.Diagnosis,
                Name = v.Patient.Name,
                Surname = v.Patient.Surname,
                Patronymic = v.Patient.Patronymic
            })
            .ToList();
    }
}