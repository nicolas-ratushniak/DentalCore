using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IProcedureService
{
    public Procedure Get(int id, bool includeSoftDeleted);
    public IEnumerable<Procedure> GetAll(bool includeSoftDeleted);
    public void Add(ProcedureCreateDto dto);
    public void Update(ProcedureUpdateDto dto);
    public void Delete(int id);
}