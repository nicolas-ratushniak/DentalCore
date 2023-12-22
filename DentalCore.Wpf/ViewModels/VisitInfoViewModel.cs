using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DentalCore.Domain.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class VisitInfoViewModel : BaseViewModel
{
    private readonly IVisitService _visitService;
    private readonly IProcedureService _procedureService;
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
        IProcedureService procedureService)
    {
        _patientId = id;
        _visitService = visitService;
        _procedureService = procedureService;
        TreatmentItems = new ObservableCollection<TreatmentItemReadOnlyListItemViewModel>();

        LoadedCommand = new AsyncRelayCommand(LoadData);
    }

    private async Task LoadData()
    {
        var visit = await _visitService.GetAsync(_patientId);

        Date = visit.VisitDate.ToString("dd.MM.yyyy");
        DoctorShortName = $"{visit.Doctor.Surname} {visit.Doctor.Name[0]}.";
        PatientShortName = $"{visit.Patient.Surname} {visit.Patient.Name[0]}.{visit.Patient.Patronymic[0]}.";
        Diagnosis = visit.Diagnosis;
        TotalSum = visit.TotalPrice;
        HasPayed = visit.AlreadyPayed;

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