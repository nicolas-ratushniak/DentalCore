using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Data.Models;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class VisitCreateViewModel : BaseViewModel
{
    private readonly ObservableCollection<TreatmentItemListItemViewModel> _treatmentItems;
    private readonly INavigationService _navigationService;
    private readonly IVisitService _visitService;
    private readonly IUserService _userService;
    private readonly IProcedureService _procedureService;
    private readonly int _patientId;
    private DoctorListItemViewModel? _selectedDoctor;
    private string? _diagnosis;
    private int _discountPercent;
    private int _firstPayment;
    private int _priceWithoutDiscount;
    private int _totalSum;
    private string _doctorSearchFilter = string.Empty;
    private string _treatmentItemSelectionFilter = string.Empty;
    private string? _errorMessage;
    private TreatmentItemListItemViewModel? _selectedTreatmentItem;
    private bool _isDoctorListVisible;
    private bool _isTreatmentItemListVisible;

    public ICommand RemoveTreatmentItemCommand { get; }
    public ICommand UpdatePriceCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SubmitCommand { get; }

    public ICollectionView DoctorCollectionView { get; }
    public ICollectionView SelectedTreatmentItemCollectionView { get; }
    public ICollectionView NonSelectedTreatmentItemCollectionView { get; }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    #region SearchAndFilters

    public string DoctorSearchFilter
    {
        get => _doctorSearchFilter;
        set
        {
            if (value == _doctorSearchFilter) return;
            _doctorSearchFilter = value;

            OnPropertyChanged();
            OnDoctorFilterChanged();
        }
    }

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

    public DoctorListItemViewModel? SelectedDoctor
    {
        get => _selectedDoctor;
        set
        {
            if (Equals(value, _selectedDoctor)) return;
            _selectedDoctor = value;

            OnPropertyChanged();
            OnSelectedDoctorChanged();
            CommandManager.InvalidateRequerySuggested();
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

    public bool IsDoctorListVisible
    {
        get => _isDoctorListVisible;
        set
        {
            if (value == _isDoctorListVisible) return;
            _isDoctorListVisible = value;
            OnPropertyChanged();
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

    #endregion

    #region UiProperties

    public string? Diagnosis
    {
        get => _diagnosis;
        set
        {
            if (value == _diagnosis) return;
            _diagnosis = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public int DiscountPercent
    {
        get => _discountPercent;
        set
        {
            if (value == _discountPercent) return;
            _discountPercent = value;

            OnPropertyChanged();
            UpdatePrice();
        }
    }

    public int FirstPayment
    {
        get => _firstPayment;
        set
        {
            if (value == _firstPayment) return;
            _firstPayment = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    #endregion

    public int PriceWithoutDiscount
    {
        get => _priceWithoutDiscount;
        private set
        {
            if (value == _priceWithoutDiscount) return;
            _priceWithoutDiscount = value;

            OnPropertyChanged();
        }
    }

    public int TotalSum
    {
        get => _totalSum;
        private set
        {
            if (value == _totalSum) return;
            _totalSum = value;
            OnPropertyChanged();
        }
    }

    public VisitCreateViewModel(
        int id,
        INavigationService navigationService,
        IVisitService visitService,
        IUserService userService,
        IPatientService patientService,
        IProcedureService procedureService)
    {
        _navigationService = navigationService;
        _visitService = visitService;
        _userService = userService;
        _procedureService = procedureService;
        _patientId = id;

        if (patientService.GetAll().All(p => p.Id != id))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        DoctorCollectionView = CollectionViewSource.GetDefaultView(GetDoctors());
        DoctorCollectionView.Filter = o => o is DoctorListItemViewModel d &&
                                           (d.Surname.ToLower().StartsWith(DoctorSearchFilter.ToLower()) ||
                                            d.Name.ToLower().StartsWith(DoctorSearchFilter.ToLower()));

        _treatmentItems = new ObservableCollection<TreatmentItemListItemViewModel>(GetTreatmentItems());

        NonSelectedTreatmentItemCollectionView = new CollectionViewSource { Source = _treatmentItems }.View;
        NonSelectedTreatmentItemCollectionView.Filter = o => o is TreatmentItemListItemViewModel t &&
                                                             !t.IsSelected &&
                                                             t.Name.ToLower()
                                                                 .Contains(TreatmentItemSelectionFilter.ToLower());

        SelectedTreatmentItemCollectionView = new CollectionViewSource { Source = _treatmentItems }.View;
        SelectedTreatmentItemCollectionView.Filter = o => o is TreatmentItemListItemViewModel t && t.IsSelected;

        CancelCommand = new RelayCommand<object>(_ => navigationService.NavigateTo(ViewType.PatientInfo, id));
        SubmitCommand = new RelayCommand<object>(AddVisit_Execute, AddVisit_CanExecute);

        RemoveTreatmentItemCommand = new RelayCommand<int>(itemId =>
        {
            var item = _treatmentItems.Single(t => t.Id == itemId);
            item.IsSelected = false;

            SelectedTreatmentItemCollectionView.Refresh();
            NonSelectedTreatmentItemCollectionView.Refresh();
            UpdatePrice();
        });

        UpdatePriceCommand = new RelayCommand<object>(_ => UpdatePrice());
    }

    private bool AddVisit_CanExecute(object obj)
    {
        return SelectedDoctor != null && _treatmentItems.Any(t => t.IsSelected);
    }

    private void AddVisit_Execute(object obj)
    {
        var items = _treatmentItems
            .Where(t => t.IsSelected && t.Quantity > 0)
            .Select(t => new TreatmentItemDto
            {
                ProcedureId = t.Id,
                Quantity = t.Quantity
            });

        var dto = new VisitCreateDto
        {
            PatientId = _patientId,
            DoctorId = SelectedDoctor!.Id,
            DiscountPercent = DiscountPercent,
            FirstPayment = FirstPayment,
            Diagnosis = Diagnosis,
            Date = DateTime.Now,
            TreatmentItems = items
        };

        try
        {
            _visitService.Add(dto);
            _navigationService.NavigateTo(ViewType.PatientInfo, _patientId);
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void OnDoctorFilterChanged()
    {
        var filter = DoctorSearchFilter;

        if (_selectedDoctor is null)
        {
            IsDoctorListVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedDoctor.FullName)
            {
                IsDoctorListVisible = false;
                return;
            }

            IsDoctorListVisible = true;
            _selectedDoctor = null;
        }

        DoctorCollectionView.Refresh();
    }

    private void OnTreatmentItemFilterChanged()
    {
        if (_selectedTreatmentItem is null)
        {
            IsTreatmentItemListVisible = !string.IsNullOrEmpty(TreatmentItemSelectionFilter);
        }

        NonSelectedTreatmentItemCollectionView.Refresh();
    }

    private void OnSelectedDoctorChanged()
    {
        if (_selectedDoctor is null)
        {
            return;
        }

        DoctorSearchFilter = _selectedDoctor.FullName;
        IsDoctorListVisible = false;
    }

    private void OnSelectedTreatmentItemChanged()
    {
        IsTreatmentItemListVisible = false;

        if (_selectedTreatmentItem is null)
        {
            return;
        }

        var item = _treatmentItems
            .Single(t => t.Id == _selectedTreatmentItem.Id);

        item.Quantity = 1;
        item.IsSelected = true;

        TreatmentItemSelectionFilter = string.Empty;
        SelectedTreatmentItemCollectionView.Refresh();
        UpdatePrice();
    }

    private void UpdatePrice()
    {
        var items = _treatmentItems
            .Where(t => t.IsSelected && t.Quantity > 0)
            .Select(t => new TreatmentItemDto
            {
                ProcedureId = t.Id,
                Quantity = t.Quantity
            })
            .ToList();

        PriceWithoutDiscount = _visitService.CalculateTotalPrice(items, 0);
        TotalSum = _visitService.CalculateTotalPrice(items, DiscountPercent);
        FirstPayment = TotalSum;
    }

    private IEnumerable<DoctorListItemViewModel> GetDoctors()
    {
        return _userService.GetAll()
            .Where(u => u.Role == UserRole.Doctor)
            .Select(d => new DoctorListItemViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Surname = d.Surname
            });
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