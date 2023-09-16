using DentalCore.Domain.Dto;
using DentalCore.Domain.Models;

namespace DentalCore.Domain.Services;

public interface IProcedureService
{
    public Procedure Get(int id);
    public IEnumerable<Procedure> GetAll();
    public void Add(ProcedureCreateDto dto);
    public void Update(ProcedureUpdateDto dto);
    public void Delete(int id);
}