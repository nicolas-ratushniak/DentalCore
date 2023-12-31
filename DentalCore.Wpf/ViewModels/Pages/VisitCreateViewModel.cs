using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Components;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class VisitCreateViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IVisitService _visitService;
    private readonly IPatientService _patientService;
    private readonly IUserService _userService;
    private readonly IProcedureService _procedureService;
    private readonly IPaymentService _paymentService;
    private readonly int _patientId;

    private string? _diagnosis;
    private int _firstPayment;
    private int _totalSum;
    private string? _errorMessage;
    private string _patientInfo;

    public ICommand CancelCommand { get; }
    public ICommand SubmitCommand { get; }

    public DoctorSelectorViewModel DoctorSelector { get; }
    public TreatmentMultiSelectorViewModel TreatmentMultiSelector { get; }

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

    public string PatientInfo
    {
        get => _patientInfo;
        set
        {
            if (value == _patientInfo) return;
            _patientInfo = value;
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
        IPatientService patientService,
        IUserService userService,
        IProcedureService procedureService,
        IPaymentService paymentService)
    {
        _navigationService = navigationService;
        _visitService = visitService;
        _patientService = patientService;
        _userService = userService;
        _procedureService = procedureService;
        _paymentService = paymentService;
        _patientId = id;

        DoctorSelector = new DoctorSelectorViewModel();

        TreatmentMultiSelector = new TreatmentMultiSelectorViewModel();
        TreatmentMultiSelector.SelectedTreatmentSetChanged += OnSelectedTreatmentsMultiChanged;

        CancelCommand = new RelayCommand(() => navigationService.NavigateTo(PageType.PatientInfo, id));
        SubmitCommand = new AsyncRelayCommand(AddVisit_Execute);
    }

    public override void Dispose()
    {
        TreatmentMultiSelector.SelectedTreatmentSetChanged -= OnSelectedTreatmentsMultiChanged;
        base.Dispose();
    }

    public override async Task LoadDataAsync()
    {
        var patient = await _patientService.GetAsync(_patientId);
        PatientInfo = $"{patient.Surname} {patient.Name} {patient.Patronymic}";

        DoctorSelector.Doctors.Clear();

        foreach (var doctor in await GetDoctorsAsync())
        {
            DoctorSelector.Doctors.Add(doctor);
        }

        TreatmentMultiSelector.TreatmentItems.Clear();

        foreach (var item in await GetTreatmentItemsAsync())
        {
            TreatmentMultiSelector.TreatmentItems.Add(item);
        }
    }

    private async Task AddVisit_Execute()
    {
        if (DoctorSelector.SelectedDoctor is null || !TreatmentMultiSelector.HasSelectedItems)
        {
            ErrorMessage = "Заповніть всі необхідні поля";
            return;
        }

        var items = TreatmentMultiSelector.GetSelectedTreatmentItems();

        var dto = new VisitCreateDto
        {
            PatientId = _patientId,
            DoctorId = DoctorSelector.SelectedDoctor!.Id,
            DiscountPercent = 0,
            FirstPayment = FirstPayment,
            Diagnosis = Diagnosis,
            Date = DateTime.Now,
            TreatmentItems = items
        };

        try
        {
            await _visitService.AddAsync(dto);
            _navigationService.NavigateTo(PageType.PatientInfo, _patientId);
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task OnSelectedTreatmentsMultiChanged(object? sender, EventArgs e)
    {
        var items = TreatmentMultiSelector.GetSelectedTreatmentItems().ToList();

        (TotalSum, _) = await _paymentService.CalculateTotalWithDiscountAsync(items, 0);
        FirstPayment = TotalSum;
    }

    private async Task<IEnumerable<DoctorListItemViewModel>> GetDoctorsAsync()
    {
        return (await _userService.GetAllAsync())
            .Where(u => u.Role == UserRole.Doctor)
            .Select(d => new DoctorListItemViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Surname = d.Surname
            });
    }

    private async Task<IEnumerable<TreatmentItemListItemViewModel>> GetTreatmentItemsAsync()
    {
        return (await _procedureService.GetAllAsync())
            .Select(p => new TreatmentItemListItemViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Quantity = 0,
                Price = p.Price
            });
    }
}