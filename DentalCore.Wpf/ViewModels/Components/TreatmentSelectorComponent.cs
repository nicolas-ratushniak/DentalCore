using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Components;

public class TreatmentSelectorComponent : BaseViewModel
{
    public event EventHandler SelectedTreatmentSetChanged;
    
    private readonly IProcedureService _procedureService;
    private readonly ObservableCollection<TreatmentItemListItemViewModel> _treatmentItems;
    
    private TreatmentItemListItemViewModel? _selectedTreatmentItem;
    private string _treatmentItemSelectionFilter = string.Empty;
    private bool _isTreatmentItemListVisible;
    private bool _canSelectTreatmentItem = true;
    
    public ICommand RemoveTreatmentItemCommand { get; }
    public ICommand UpdatePriceCommand { get; }
    
    public ICollectionView SelectedTreatmentItemCollectionView { get; }
    public ICollectionView NonSelectedTreatmentItemCollectionView { get; }

    public bool HasSelectedItems => _treatmentItems.Any(t => t.IsSelected && t.Quantity > 0);
    
    public string TreatmentItemSelectionFilter
    {
        get => _treatmentItemSelectionFilter;
        set
        {
            if (value == _treatmentItemSelectionFilter) return;
            _treatmentItemSelectionFilter = value;

            OnPropertyChanged();
            OnTreatmentItemFilterChanged();
        }
    }
    
    public TreatmentItemListItemViewModel? SelectedTreatmentItem
    {
        get => _selectedTreatmentItem;
        set
        {
            if (Equals(value, _selectedTreatmentItem) || !_canSelectTreatmentItem) return;
            _selectedTreatmentItem = value;

            OnPropertyChanged();
            OnSelectedTreatmentItemChanged();
        }
    }
    
    public bool IsTreatmentItemListVisible
    {
        get => _isTreatmentItemListVisible;
        set
        {
            if (value == _isTreatmentItemListVisible) return;
            _isTreatmentItemListVisible = value;
            OnPropertyChanged();
        }
    }

    public TreatmentSelectorComponent(IProcedureService procedureService)
    {
        _procedureService = procedureService;
        _treatmentItems = new ObservableCollection<TreatmentItemListItemViewModel>(GetTreatmentItems());

        NonSelectedTreatmentItemCollectionView = new CollectionViewSource { Source = _treatmentItems }.View;

        NonSelectedTreatmentItemCollectionView.Filter = o =>
            o is TreatmentItemListItemViewModel t && !t.IsSelected &&
            t.Name.ToLower().Contains(TreatmentItemSelectionFilter.ToLower());

        SelectedTreatmentItemCollectionView = new CollectionViewSource { Source = _treatmentItems }.View;

        SelectedTreatmentItemCollectionView.Filter = o =>
            o is TreatmentItemListItemViewModel t && t.IsSelected;
        
        RemoveTreatmentItemCommand = new RelayCommand<int>(itemId =>
        {
            var item = _treatmentItems.Single(t => t.Id == itemId);
            item.Quantity = 0;
            item.IsSelected = false;

            SelectedTreatmentItemCollectionView.Refresh();
            NonSelectedTreatmentItemCollectionView.Refresh();

            SelectedTreatmentSetChanged?.Invoke(this, EventArgs.Empty);
        });
        
        UpdatePriceCommand = new RelayCommand<object>(_ => 
            SelectedTreatmentSetChanged?.Invoke(this, EventArgs.Empty));
    }

    public IEnumerable<TreatmentItemDto> GetSelectedTreatmentItems()
    {
        return _treatmentItems
            .Where(t => t.IsSelected && t.Quantity > 0)
            .Select(t => new TreatmentItemDto
            {
                ProcedureId = t.Id,
                Quantity = t.Quantity
            });
    }
    
    private void OnTreatmentItemFilterChanged()
    {
        if (string.IsNullOrEmpty(TreatmentItemSelectionFilter))
        {
            IsTreatmentItemListVisible = false;
        }
        else
        {
            NonSelectedTreatmentItemCollectionView.Refresh();
            IsTreatmentItemListVisible = true;
            _canSelectTreatmentItem = true;
        }
    }
    
    private void OnSelectedTreatmentItemChanged()
    {
        TreatmentItemSelectionFilter = string.Empty;
        _canSelectTreatmentItem = false;
        
        if (_selectedTreatmentItem is null)
        {
            return;
        }
        
        var item = _treatmentItems
            .Single(t => t.Id == _selectedTreatmentItem.Id);

        item.Quantity = 1;
        item.IsSelected = true;

        SelectedTreatmentItemCollectionView.Refresh();
        NonSelectedTreatmentItemCollectionView.Refresh();
        SelectedTreatmentSetChanged?.Invoke(this, EventArgs.Empty);
        
        _selectedTreatmentItem = null;
    }
    
    private IEnumerable<TreatmentItemListItemViewModel> GetTreatmentItems()
    {
        return _procedureService.GetAll()
            .Select(p => new TreatmentItemListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Quantity = 0,
                Price = p.Price
            });
    }
}