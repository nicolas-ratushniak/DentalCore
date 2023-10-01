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

    public Procedure Get(int id)
    {
        return _context.Procedures
            .Where(p => !p.IsDeleted)
            .SingleOrDefault(p => p.Id == id) ?? throw new EntityNotFoundException();
    }

    public Procedure GetIncludeSoftDeleted(int id)
    {
        return _context.Procedures.Find(id) ?? throw new EntityNotFoundException();
    }

    public IEnumerable<Procedure> GetAll()
    {
        return _context.Procedures.Where(p => !p.IsDeleted);
    }

    public IEnumerable<Procedure> GetAllIncludeSoftDeleted()
    {
        return _context.Procedures.ToList();
    }

    public int Add(ProcedureCreateDto dto)
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
            IsDiscountAllowed = dto.IsDiscountAllowed
        };

        _context.Procedures.Add(procedure);
        _context.SaveChanges();

        return procedure.Id;
    }

    public void Update(ProcedureUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Procedures.Any(p => p.Name == dto.Name && p.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є процедура з такою назвою");
        }

        var procedure = Get(dto.Id);

        procedure.Name = dto.Name;
        procedure.Price = dto.Price;
        procedure.IsDiscountAllowed = dto.IsDiscountAllowed;

        _context.Procedures.Update(procedure);
        _context.SaveChanges();
    }

    public void SoftDelete(int id)
    {
        var procedure = Get(id);

        procedure.IsDeleted = true;

        _context.Procedures.Update(procedure);
        _context.SaveChanges();
    }
}