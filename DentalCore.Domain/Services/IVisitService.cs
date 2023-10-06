using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IVisitService
{
    public Visit Get(int id);
    public IEnumerable<Visit> GetAll();
    public int Add(VisitCreateDto dto);
    public IEnumerable<TreatmentItem> GetTreatmentItems(int id);
}