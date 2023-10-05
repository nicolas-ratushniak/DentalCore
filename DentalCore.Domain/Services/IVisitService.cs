using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IVisitService
{
    public Visit Get(int id);
    public IEnumerable<Visit> GetAll();
    public int Add(VisitCreateDto dto);
    public void AddPayment(int id, int sum);
    public int GetDebt(int id);
    public int GetMoneyPayed(int id);

    public int CalculateTotalWithDiscount(IEnumerable<TreatmentItemDto> selectedTreatmentItems, int discountPercent,
        out int discountSum);

    public IEnumerable<TreatmentItem> GetTreatmentItems(int id);
}