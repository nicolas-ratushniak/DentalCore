using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class ProcedureService : IProcedureService
{
    private readonly AppDbContext _context;

    public ProcedureService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Procedure> GetAsync(int id)
    {
        return await _context.Procedures
            .Where(p => !p.IsDeleted)
            .SingleOrDefaultAsync(p => p.Id == id) ?? throw new EntityNotFoundException();
    }

    public async Task<Procedure> GetIncludeSoftDeletedAsync(int id)
    {
        return await _context.Procedures.FindAsync(id) ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<Procedure>> GetAllAsync()
    {
        return await _context.Procedures
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<Procedure>> GetAllIncludeSoftDeletedAsync()
    {
        return await _context.Procedures.ToListAsync();
    }

    public async Task<int> AddAsync(ProcedureCreateDto dto)
    {
        var createdOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (await _context.Procedures.AnyAsync(p => p.Name == dto.Name))
        {
            throw new ValidationException("У базі вже є процедура з такою назвою");
        }

        var procedure = new Procedure
        {
            Name = dto.Name,
            Price = dto.Price,
            IsDiscountAllowed = dto.IsDiscountAllowed,
            CreatedOn = createdOn
        };

        await _context.Procedures.AddAsync(procedure);
        await _context.SaveChangesAsync();

        return procedure.Id;
    }

    public async Task UpdateAsync(ProcedureUpdateDto dto)
    {
        var updatedOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (await _context.Procedures.AnyAsync(p =>
                p.Name == dto.Name &&
                p.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є процедура з такою назвою");
        }

        var procedure = await GetAsync(dto.Id);

        procedure.Name = dto.Name;
        procedure.Price = dto.Price;
        procedure.IsDiscountAllowed = dto.IsDiscountAllowed;
        procedure.UpdatedOn = updatedOn;

        _context.Procedures.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var deletedOn = DateTime.Now;
        var procedure = await GetAsync(id);

        procedure.IsDeleted = true;
        procedure.DeletedOn = deletedOn;

        _context.Procedures.Update(procedure);
        await _context.SaveChangesAsync();
    }
}