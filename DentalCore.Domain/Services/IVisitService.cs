using DentalCore.Domain.Dto;
using DentalCore.Domain.Models;

namespace DentalCore.Domain.Services;

public interface IVisitService
{
    public Visit Get(int id);
    public IEnumerable<Visit> GetAll();
    public void Add(VisitCreateDto dto);
    public void AddPayment(int sum);
    public int GetDebt(int id);
    public int GetMoneyPayed(int id);
    public IEnumerable<TreatmentItem> GetTreatmentItems(int id);
    public IEnumerable<Payment> GetPayments(int id);
}