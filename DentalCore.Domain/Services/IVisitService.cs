using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IVisitService
{
    public Task<Visit> GetAsync(int id);
    public Task<IEnumerable<Visit>> GetAllAsync();
    public Task<int> AddAsync(VisitCreateDto dto);
    public Task<IEnumerable<TreatmentItem>> GetTreatmentItemsAsync(int id);
}