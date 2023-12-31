﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands.Generic;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Components;

public class AllergyMultiSelectorViewModel : BaseViewModel
{
    private string _allergySelectionFilter = string.Empty;
    private bool _isAllergyListVisible;
    private AllergyListItemViewModel? _selectedAllergy;
    
    public ICommand AddAllergyCommand { get; }
    public ICommand RemoveAllergyCommand { get; }
    
    public ICollectionView SelectedAllergyCollectionView { get; }
    public ICollectionView NotSelectedAllergyCollectionView { get; }

    public ObservableCollection<AllergyListItemViewModel> Allergies { get; }

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
            if (Equals(value, _selectedAllergy)) return;
            _selectedAllergy = value;

            OnPropertyChanged();
            OnSelectedAllergyChanged();
        }
    }

    public AllergyMultiSelectorViewModel(Func<string, Task> addAllergyCallback)
    {
        Allergies = new ObservableCollection<AllergyListItemViewModel>();
        
        NotSelectedAllergyCollectionView = new CollectionViewSource { Source = Allergies }.View;
        SelectedAllergyCollectionView = new CollectionViewSource { Source = Allergies }.View;
        
        NotSelectedAllergyCollectionView.Filter = o =>
            o is AllergyListItemViewModel { IsSelected: false } a &&
            a.Name.ToLower().StartsWith(AllergySelectionFilter.ToLower());

        SelectedAllergyCollectionView.Filter = o =>
            o is AllergyListItemViewModel { IsSelected: true };

        AddAllergyCommand = new AsyncRelayCommand<string>(addAllergyCallback);
        
        RemoveAllergyCommand = new RelayCommand<int>(allergyId =>
        {
            var allergy = Allergies.Single(a => a.Id == allergyId);
            allergy.IsSelected = false;

            SelectedAllergyCollectionView.Refresh();
            NotSelectedAllergyCollectionView.Refresh();
        });
    }

    public IEnumerable<int> GetSelectedAllergiesIds()
    {
        return Allergies
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
        }
    }

    private void OnSelectedAllergyChanged()
    {
        AllergySelectionFilter = string.Empty;

        if (_selectedAllergy is null)
        {
            return;
        }

        var item = Allergies
            .Single(t => t.Id == _selectedAllergy.Id);

        item.IsSelected = true;

        SelectedAllergyCollectionView.Refresh();
        NotSelectedAllergyCollectionView.Refresh();

        SelectedAllergy = null;
    }
}