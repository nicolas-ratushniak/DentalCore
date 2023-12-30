using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DentalCore.Domain.Abstract;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class VisitInfoViewModel : BaseViewModel
{
    private readonly IVisitService _visitService;
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

    public VisitInfoViewModel(int id, IVisitService visitService)
    {
        _patientId = id;
        _visitService = visitService;
        TreatmentItems = new ObservableCollection<TreatmentItemReadOnlyListItemViewModel>();
    }

    public override async Task LoadDataAsync()
    {
        var visit = await _visitService.GetAsync(_patientId);

        Date = visit.VisitDate.ToString("dd.MM.yyyy");
        DoctorShortName = $"{visit.Doctor.Surname} {visit.Doctor.Name[0]}.";
        PatientShortName = $"{visit.Patient.Surname} {visit.Patient.Name[0]}.{visit.Patient.Patronymic[0]}.";
        Diagnosis = visit.Diagnosis;
        TotalSum = visit.TotalPrice;
        HasPayed = visit.AlreadyPayed;
        
        TreatmentItems.Clear();

        foreach (var item in await GetTreatmentItemsAsync(visit.Id))
        {
            TreatmentItems.Add(item);
        }
    }
    
    private async Task<IEnumerable<TreatmentItemReadOnlyListItemViewModel>> GetTreatmentItemsAsync(int visitId)
    {
        return (await _visitService.GetTreatmentItemsAsync(visitId))
            .Select(p => new TreatmentItemReadOnlyListItemViewModel
            {
                Name = p.Name,
                Quantity = p.Quantity,
                Price = p.Price
            });
    }
}