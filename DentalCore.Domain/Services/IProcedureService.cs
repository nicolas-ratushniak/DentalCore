using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IProcedureService
{
    public Task<Procedure> GetAsync(int id);
    public Task<Procedure> GetIncludeSoftDeletedAsync(int id);
    public Task<IEnumerable<Procedure>> GetAllAsync();
    public Task<IEnumerable<Procedure>> GetAllIncludeSoftDeletedAsync();
    public Task<int> AddAsync(ProcedureCreateDto dto);
    public Task UpdateAsync(ProcedureUpdateDto dto);
    public Task SoftDeleteAsync(int id);
}