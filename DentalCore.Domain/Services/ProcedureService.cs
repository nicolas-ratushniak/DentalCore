using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;

namespace DentalCore.Domain.Services;

public class ProcedureService : IProcedureService
{
    private readonly AppDbContext _context;

    public ProcedureService(AppDbContext context)
    {
        _context = context;
    }

    public Procedure Get(int id, bool includeSoftDeleted)
    {
        return includeSoftDeleted
            ? _context.Procedures.Find(id) ?? throw new EntityNotFoundException()
            : _context.Procedures
                .Where(p => !p.IsDeleted)
                .SingleOrDefault(p => p.Id == id) ?? throw new EntityNotFoundException();
    }

    public IEnumerable<Procedure> GetAll(bool includeSoftDeleted)
    {
        return includeSoftDeleted
            ? _context.Procedures.ToList()
            : _context.Procedures.Where(p => !p.IsDeleted);
    }

    public void Add(ProcedureCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Procedures.Any(p => p.Name == dto.Name))
        {
            throw new ValidationException("У базі вже є процедура з такою назвою");
        }

        var procedure = new Procedure
        {
            Name = dto.Name,
            Price = dto.Price,
            IsDiscountValid = dto.IsDiscountValid
        };

        _context.Procedures.Add(procedure);
        _context.SaveChanges();
    }

    public void Update(ProcedureUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Procedures.Any(p => p.Name == dto.Name && p.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є процедура з такою назвою");
        }

        var procedure = Get(dto.Id, false);

        procedure.Name = dto.Name;
        procedure.Price = dto.Price;
        procedure.IsDiscountValid = dto.IsDiscountValid;

        _context.Procedures.Update(procedure);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var procedure = Get(id, false);

        procedure.IsDeleted = true;
        
        _context.Procedures.Update(procedure);
        _context.SaveChanges();
    }
}