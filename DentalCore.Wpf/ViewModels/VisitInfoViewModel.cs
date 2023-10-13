
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class VisitInfoViewModel : BaseViewModel
{
    private readonly IVisitService _visitService;
    private readonly IPatientService _patientService;
    private readonly IUserService _userService;
    private readonly IProcedureService _procedureService;
    private readonly IPaymentService _paymentService;
    private readonly int _patientId;
    private string _date;
    private string _doctorShortName;
    private string _patientShortName;
    private string? _diagnosis;
    private int _totalSum;
    private int _hasPayed;
    
    public ObservableCollection<TreatmentItemReadOnlyListItemViewModel> TreatmentItems { get; }

    public string Date
    {
        get => _date;
        private set
        {
            if (value == _date) return;
            _date = value;
            OnPropertyChanged();
        }
    }

    public string DoctorShortName
    {
        get => _doctorShortName;
        private set
        {
            if (value == _doctorShortName) return;
            _doctorShortName = value;
            OnPropertyChanged();
        }
    }

    public string PatientShortName
    {
        get => _patientShortName;
        private set
        {
            if (value == _patientShortName) return;
            _patientShortName = value;
            OnPropertyChanged();
        }
    }

    public string? Diagnosis
    {
        get => _diagnosis;
        private set
        {
            if (value == _diagnosis) return;
            _diagnosis = value;
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

    public int HasPayed
    {
        get => _hasPayed;
        private set
        {
            if (value == _hasPayed) return;
            _hasPayed = value;
            OnPropertyChanged();
        }
    }

    public VisitInfoViewModel(
        int id,
        IVisitService visitService,
        IPatientService patientService,
        IUserService userService,
        IProcedureService procedureService,
        IPaymentService paymentService)
    {
        _patientId = id;
        _visitService = visitService;
        _patientService = patientService;
        _userService = userService;
        _procedureService = procedureService;
        _paymentService = paymentService;
        TreatmentItems = new ObservableCollection<TreatmentItemReadOnlyListItemViewModel>();

        LoadedCommand = new AsyncRelayCommand(LoadData);
    }

    private async Task LoadData()
    {
        var visit = await _visitService.GetAsync(_patientId);
        var doctor = await _userService.GetIncludeSoftDeletedAsync(visit.DoctorId);
        var patient = await _patientService.GetAsync(visit.PatientId);

        Date = visit.CreatedOn.ToString("dd.MM.yyyy");
        DoctorShortName = $"{doctor.Surname} {doctor.Name[0]}.";
        PatientShortName = $"{patient.Surname} {patient.Name[0]}.{patient.Patronymic[0]}.";
        Diagnosis = visit.Diagnosis;
        TotalSum = visit.TotalPrice;
        HasPayed = await _paymentService.GetMoneyPayedForVisitAsync(_patientId);

        var procedures = (await _procedureService.GetAllIncludeSoftDeletedAsync()).ToList();

        foreach (var item in await _visitService.GetTreatmentItemsAsync(visit.Id))
        {
            TreatmentItems.Add(new TreatmentItemReadOnlyListItemViewModel
            {
                Name = procedures.Single(p => p.Id == item.ProcedureId).Name,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }
    }
}