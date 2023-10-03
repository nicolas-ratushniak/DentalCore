using System.Collections.Generic;
using System.Linq;
using DentalCore.Domain.Services;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class VisitInfoViewModel : BaseViewModel
{
    public string Date { get; }
    public string DoctorShortName { get; }
    public string PatientShortName { get; }
    public string? Diagnosis { get; }
    public int TotalSum { get; }
    public int HasPayed { get; }
    public IEnumerable<TreatmentItemReadOnlyListItemViewModel> TreatmentItems { get; }
    
    public VisitInfoViewModel(
        int id,
        IVisitService visitService,
        IPatientService patientService,
        IUserService userService,
        IProcedureService procedureService)
    {
        var visit = visitService.Get(id);
        var doctor = userService.GetIncludeSoftDeleted(visit.DoctorId);
        var patient = patientService.Get(visit.PatientId);
        
        Date = visit.Date.ToString("dd.MM.yyyy");
        DoctorShortName = $"{doctor.Surname} {doctor.Name[0]}.";
        PatientShortName = $"{patient.Surname} {patient.Name[0]}.{patient.Patronymic[0]}.";
        Diagnosis = visit.Diagnosis;
        TotalSum = visit.TotalPrice;
        HasPayed = visitService.GetMoneyPayed(id);

        var procedures = procedureService.GetAllIncludeSoftDeleted().ToList();

        TreatmentItems = visitService.GetTreatmentItems(id)
            .Select(t => new TreatmentItemReadOnlyListItemViewModel
            {
                Name = procedures.Single(p => p.Id == t.ProcedureId).Name,
                Quantity = t.Quantity,
                Price = t.Price
            });
    }
}