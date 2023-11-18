using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IVisitService
{
    public Task<Visit> GetAsync(int id);
    public Task<IEnumerable<Visit>> GetAllAsync();
    public Task<int> AddAsync(VisitCreateDto dto);
    public Task<IEnumerable<TreatmentItem>> GetTreatmentItemsAsync(int id);
}