using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Services;

public interface IPaymentService
{
    public Payment Get(int id);
    public IEnumerable<Payment> GetAll();
    public int GetPatientDebt(int patientId);
    public int GetVisitDebt(int visitId);
    public void PayPatientDebt(int patientId);
    public void AddVisitPayment(int visitId, int sum);
    public int GetMoneyPayedForVisit(int visitId);

    public int CalculateTotalWithDiscount(IEnumerable<TreatmentItemDto> selectedTreatmentItems, int discountPercent,
        out int discountSum);
}