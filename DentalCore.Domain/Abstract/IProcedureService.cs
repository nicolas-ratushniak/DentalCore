using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IProcedureService
{
    public Task<ProcedureDto> GetAsync(int id);
    public Task<IEnumerable<ProcedureDto>> GetAllAsync();
    public Task<IEnumerable<TreatmentItemDto>> GetVisitTreatmentItemsAsync(int visitId);
    public Task<IEnumerable<ProcedureDto>> GetAllIncludeSoftDeletedAsync();
    public Task<int> AddAsync(ProcedureCreateDto dto);
    public Task UpdateAsync(ProcedureUpdateDto dto);
    public Task SoftDeleteAsync(int id);
}