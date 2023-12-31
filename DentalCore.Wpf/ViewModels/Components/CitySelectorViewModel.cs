using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands.Generic;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Components;

public class CitySelectorViewModel : BaseViewModel
{
    private string _citySearchFilter = string.Empty;
    private bool _isCityListVisible;
    private CityListItemViewModel? _selectedCity;
    
    public ObservableCollection<CityListItemViewModel> Cities { get; }
    public ICollectionView CityCollectionView { get; }
    public ICommand AddCityCommand { get; }
    
    public string CitySearchFilter
    {
        get => _citySearchFilter;
        set
        {
            if (value == _citySearchFilter) return;
            _citySearchFilter = value;

            OnPropertyChanged();
            OnCityFilterChanged();
        }
    }

    public bool IsCityListVisible
    {
        get => _isCityListVisible;
        set
        {
            if (value == _isCityListVisible) return;
            _isCityListVisible = value;
            OnPropertyChanged();
        }
    }

    public CityListItemViewModel? SelectedCity
    {
        get => _selectedCity;
        set
        {
            if (Equals(value, _selectedCity)) return;
            _selectedCity = value;

            if (_selectedCity is not null)
            {
                CitySearchFilter = _selectedCity.Name;
                IsCityListVisible = false;
            }

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public CitySelectorViewModel(Func<string, Task> addCityCallback)
    {
        AddCityCommand = new AsyncRelayCommand<string>(addCityCallback);
        
        Cities = new ObservableCollection<CityListItemViewModel>();
        CityCollectionView = CollectionViewSource.GetDefaultView(Cities);

        CityCollectionView.Filter = o =>
            o is CityListItemViewModel c &&
            c.Name.ToLower().StartsWith(CitySearchFilter.ToLower());
    }
    
    private void OnCityFilterChanged()
    {
        var filter = CitySearchFilter;

        if (_selectedCity is null)
        {
            IsCityListVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedCity.Name)
            {
                IsCityListVisible = false;
                return;
            }

            IsCityListVisible = true;
            _selectedCity = null;
        }

        CityCollectionView.Refresh();
    }
}