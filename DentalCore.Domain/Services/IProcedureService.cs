using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IProcedureService
{
    public Procedure Get(int id);
    public Procedure GetIncludeSoftDeleted(int id);
    public IEnumerable<Procedure> GetAll();
    public IEnumerable<Procedure> GetAllIncludeSoftDeleted();
    public void Add(ProcedureCreateDto dto);
    public void Update(ProcedureUpdateDto dto);
    public void SoftDelete(int id);
}