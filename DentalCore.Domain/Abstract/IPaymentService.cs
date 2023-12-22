using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IPaymentService
{
    public Task<PaymentDto> GetAsync(int id);
    public Task<IEnumerable<PaymentDto>> GetAllAsync(DateTime from, DateTime to);
    public Task AddAsync(PaymentCreateDto dto);
    public Task PayPatientDebtAsync(int patientId, DateTime paymentDate);
    public Task<int> GetPatientDebtAsync(int patientId);
    public Task<int> GetVisitDebtAsync(int visitId);
    public Task<int> GetMoneyPayedForVisitAsync(int visitId);

    public Task<(int, int)> CalculateTotalWithDiscountAsync(
        IEnumerable<TreatmentItemDto> selectedTreatmentItems,
        int discountPercent);
}