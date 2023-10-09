﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Components;

public class AllergySelectorComponent : BaseViewModel
{
    private readonly IPatientService _patientService;
    private readonly ICommonService _commonService;
    private readonly ObservableCollection<AllergyListItemViewModel> _allergies;
    
    private bool _canSelectAllergy = true;
    private string _allergySelectionFilter = string.Empty;
    private bool _isAllergyListVisible;
    private AllergyListItemViewModel? _selectedAllergy;
    
    public ICommand RemoveAllergyCommand { get; }
    
    public ICollectionView SelectedAllergyCollectionView { get; }
    public ICollectionView NotSelectedAllergyCollectionView { get; }
    
    public string AllergySelectionFilter
    {
        get => _allergySelectionFilter;
        set
        {
            if (value == _allergySelectionFilter) return;
            _allergySelectionFilter = value;

            OnPropertyChanged();
            OnAllergyFilterChanged();
        }
    }
    
    public bool IsAllergyListVisible
    {
        get => _isAllergyListVisible;
        set
        {
            if (value == _isAllergyListVisible) return;
            _isAllergyListVisible = value;
            OnPropertyChanged();
        }
    }
    
    public AllergyListItemViewModel? SelectedAllergy
    {
        get => _selectedAllergy;
        set
        {
            if (Equals(value, _selectedAllergy) || !_canSelectAllergy) return;
            _selectedAllergy = value;

            OnPropertyChanged();
            OnSelectedAllergyChanged();
        }
    }

    public AllergySelectorComponent(int? patientId, IPatientService patientService, ICommonService commonService)
    {
        _patientService = patientService;
        _commonService = commonService;

        var allergySource = patientId is null
            ? GetAllergies()
            : GetPatientAllergies((int)patientId);
        
        _allergies = new ObservableCollection<AllergyListItemViewModel>(allergySource);
        
        NotSelectedAllergyCollectionView = new CollectionViewSource { Source = _allergies }.View;

        NotSelectedAllergyCollectionView.Filter = o =>
            o is AllergyListItemViewModel a && !a.IsSelected &&
            a.Name.ToLower().StartsWith(AllergySelectionFilter.ToLower());
        
        SelectedAllergyCollectionView = new CollectionViewSource { Source = _allergies }.View;

        SelectedAllergyCollectionView.Filter = o =>
            o is AllergyListItemViewModel a && a.IsSelected;
        
        RemoveAllergyCommand = new RelayCommand<int>(allergyId =>
        {
            var allergy = _allergies.Single(a => a.Id == allergyId);
            allergy.IsSelected = false;

            SelectedAllergyCollectionView.Refresh();
            NotSelectedAllergyCollectionView.Refresh();
        });
    }

    public IEnumerable<int> GetSelectedAllergiesIds()
    {
        return _allergies
            .Where(a => a.IsSelected)
            .Select(a => a.Id);
    }
    
    private void OnAllergyFilterChanged()
    {
        if (string.IsNullOrEmpty(AllergySelectionFilter))
        {
            IsAllergyListVisible = false;
        }
        else
        {
            NotSelectedAllergyCollectionView.Refresh();
            IsAllergyListVisible = true;
            _canSelectAllergy = true;
        }
    }

    private void OnSelectedAllergyChanged()
    {
        AllergySelectionFilter = string.Empty;
        _canSelectAllergy = false;

        if (_selectedAllergy is null)
        {
            return;
        }

        var item = _allergies
            .Single(t => t.Id == _selectedAllergy.Id);

        item.IsSelected = true;

        SelectedAllergyCollectionView.Refresh();
        NotSelectedAllergyCollectionView.Refresh();

        _selectedAllergy = null;
    }
    
    private IEnumerable<AllergyListItemViewModel> GetAllergies()
    {
        return _commonService.GetAllergies()
            .Select(a => new AllergyListItemViewModel
            {
                Id = a.Id,
                IsSelected = false,
                Name = a.Name
            });
    }
    
    private IEnumerable<AllergyListItemViewModel> GetPatientAllergies(int patientId)
    {
        var patientAllergies = _patientService.GetAllergies(patientId);

        return _commonService.GetAllergies()
            .Select(a => new AllergyListItemViewModel
            {
                Id = a.Id,
                IsSelected = patientAllergies.Any(allergy => a.Id == allergy.Id),
                Name = a.Name
            });
    }
}