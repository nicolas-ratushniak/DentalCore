using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Services;

public interface IPaymentService
{
    public Task<Payment> GetAsync(int id);
    public Task<IEnumerable<Payment>> GetAllAsync(DateTime from, DateTime to);
    public Task<int> GetPatientDebtAsync(int patientId);
    public Task<int> GetVisitDebtAsync(int visitId);
    public Task PayPatientDebtAsync(int patientId);
    public Task AddVisitPaymentAsync(int visitId, int sum);
    public Task<int> GetMoneyPayedForVisitAsync(int visitId);

    public Task<(int, int)> CalculateTotalWithDiscountAsync(
        IEnumerable<TreatmentItemDto> selectedTreatmentItems,
        int discountPercent);
}