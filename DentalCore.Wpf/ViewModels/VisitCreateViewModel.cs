using System;
using System.Collections.Generic;
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
using DentalCore.Wpf.ViewModels.Components;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class VisitCreateViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IVisitService _visitService;
    private readonly IUserService _userService;
    private readonly IPaymentService _paymentService;
    private readonly int _patientId;

    private DoctorListItemViewModel? _selectedDoctor;
    private string _doctorSearchFilter = string.Empty;
    private string? _diagnosis;
    private int _firstPayment;
    private int _totalSum;
    private string? _errorMessage;
    private bool _isDoctorListVisible;

    public ICommand CancelCommand { get; }
    public ICommand SubmitCommand { get; }

    public ICollectionView DoctorCollectionView { get; }
    public TreatmentSelectorComponent TreatmentSelector { get; }

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
        IProcedureService procedureService,
        IPaymentService paymentService)
    {
        _navigationService = navigationService;
        _visitService = visitService;
        _userService = userService;
        _paymentService = paymentService;
        _patientId = id;

        if (patientService.GetAll().All(p => p.Id != id))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        TreatmentSelector = new TreatmentSelectorComponent(procedureService);
        TreatmentSelector.SelectedTreatmentSetChanged += OnSelectedTreatmentsChanged;

        var doctors = GetDoctors();
        
        DoctorCollectionView = CollectionViewSource.GetDefaultView(doctors);
        DoctorCollectionView.Filter = o =>
        {
            if (o is DoctorListItemViewModel d)
            {
                return d.Surname.ToLower().StartsWith(DoctorSearchFilter.ToLower()) ||
                       d.Name.ToLower().StartsWith(DoctorSearchFilter.ToLower());
            }

            return false;
        };

        CancelCommand = new RelayCommand<object>(_ => navigationService.NavigateTo(ViewType.PatientInfo, id));
        SubmitCommand = new RelayCommand<object>(AddVisit_Execute, AddVisit_CanExecute);
    }

    public override void Dispose()
    {
        TreatmentSelector.SelectedTreatmentSetChanged -= OnSelectedTreatmentsChanged;
        base.Dispose();
    }

    private bool AddVisit_CanExecute(object obj)
    {
        return SelectedDoctor != null && TreatmentSelector.HasSelectedItems;
    }

    private void AddVisit_Execute(object obj)
    {
        var items = TreatmentSelector.GetSelectedTreatmentItems();

        var dto = new VisitCreateDto
        {
            PatientId = _patientId,
            DoctorId = SelectedDoctor!.Id,
            DiscountPercent = 0,
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

    private void OnSelectedDoctorChanged()
    {
        if (_selectedDoctor is null)
        {
            return;
        }

        DoctorSearchFilter = _selectedDoctor.FullName;
        IsDoctorListVisible = false;
    }

    private void OnSelectedTreatmentsChanged(object? sender, EventArgs e)
    {
        var items = TreatmentSelector.GetSelectedTreatmentItems().ToList();

        TotalSum = _paymentService.CalculateTotalWithDiscount(items, 0, out _);
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
}