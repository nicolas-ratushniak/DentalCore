using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IVisitService
{
    public Visit Get(int id);
    public IEnumerable<Visit> GetAll();
    public void Add(VisitCreateDto dto);
    public void AddPayment(int id, int sum);
    public int GetDebt(int id);
    public int GetMoneyPayed(int id);
    public int CalculateTotalPrice(int sum, int discountPercent);
    public IEnumerable<TreatmentItem> GetTreatmentItems(int id);
}