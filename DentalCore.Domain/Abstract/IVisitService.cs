using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IVisitService
{
    public Task<VisitDto> GetAsync(int id);
    public Task<IEnumerable<VisitDto>> GetAllAsync();
    public Task<IEnumerable<VisitRichDto>> GetAllRichAsync();
    public Task<int> AddAsync(VisitCreateDto dto);
    public Task<IEnumerable<TreatmentItem>> GetTreatmentItemsAsync(int id);
}