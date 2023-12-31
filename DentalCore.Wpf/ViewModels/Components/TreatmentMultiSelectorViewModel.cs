using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Commands.Generic;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Components;

public class TreatmentMultiSelectorViewModel : BaseViewModel
{
    public delegate Task AsyncEventHandler(object sender, EventArgs e);
    public event AsyncEventHandler SelectedTreatmentSetChanged;

    private TreatmentItemListItemViewModel? _selectedTreatmentItem;
    private string _treatmentItemSelectionFilter = string.Empty;
    private bool _isTreatmentItemListVisible;

    public ICommand RemoveTreatmentItemCommand { get; }
    public ICommand UpdatePriceCommand { get; }

    public ICollectionView SelectedTreatmentItemCollectionView { get; }
    public ICollectionView NonSelectedTreatmentItemCollectionView { get; }

    public ObservableCollection<TreatmentItemListItemViewModel> TreatmentItems { get; }

    public bool HasSelectedItems => TreatmentItems.Any(t => t.IsSelected && t.Quantity > 0);

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
            if (Equals(value, _selectedTreatmentItem)) return;
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

    public TreatmentMultiSelectorViewModel()
    {
        TreatmentItems = new ObservableCollection<TreatmentItemListItemViewModel>();

        NonSelectedTreatmentItemCollectionView = new CollectionViewSource { Source = TreatmentItems }.View;

        NonSelectedTreatmentItemCollectionView.Filter = o =>
            o is TreatmentItemListItemViewModel t && !t.IsSelected &&
            t.Name.ToLower().Contains(TreatmentItemSelectionFilter.ToLower());

        SelectedTreatmentItemCollectionView = new CollectionViewSource { Source = TreatmentItems }.View;

        SelectedTreatmentItemCollectionView.Filter = o =>
            o is TreatmentItemListItemViewModel t && t.IsSelected;

        RemoveTreatmentItemCommand = new RelayCommand<int>(itemId =>
        {
            var item = TreatmentItems.Single(t => t.Id == itemId);
            item.Quantity = 0;
            item.IsSelected = false;

            SelectedTreatmentItemCollectionView.Refresh();
            NonSelectedTreatmentItemCollectionView.Refresh();
        });

        UpdatePriceCommand = new RelayCommand(() =>
            SelectedTreatmentSetChanged?.Invoke(this, EventArgs.Empty));
    }

    public IEnumerable<TreatmentItemCreateDto> GetSelectedTreatmentItems()
    {
        return TreatmentItems
            .Where(t => t is { IsSelected: true, Quantity: > 0 })
            .Select(t => new TreatmentItemCreateDto
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
        }
    }

    private void OnSelectedTreatmentItemChanged()
    {
        TreatmentItemSelectionFilter = string.Empty;

        if (_selectedTreatmentItem is null)
        {
            return;
        }

        var item = TreatmentItems
            .Single(t => t.Id == _selectedTreatmentItem.Id);

        item.Quantity = 1;
        item.IsSelected = true;

        SelectedTreatmentItemCollectionView.Refresh();
        NonSelectedTreatmentItemCollectionView.Refresh();
        SelectedTreatmentSetChanged?.Invoke(this, EventArgs.Empty);

        SelectedTreatmentItem = null;
    }
}