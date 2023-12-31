using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IVisitService
{
    public Task<VisitRichDto> GetAsync(int id);
    public Task<IEnumerable<VisitDto>> GetAllAsync();
    public Task<IEnumerable<VisitRichDto>> GetAllRichAsync(DateTime from, DateTime to);
    public Task<int> AddAsync(VisitCreateDto dto);
    public Task<IEnumerable<TreatmentItemDto>> GetTreatmentItemsAsync(int visitId);
}