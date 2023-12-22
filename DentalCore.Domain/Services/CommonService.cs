using DentalCore.Data;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class CommonService : ICommonService
{
    private readonly AppDbContext _context;

    public CommonService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DiseaseDto>> GetDiseasesAsync()
    {
        return await _context.Diseases
            .Select(d => new DiseaseDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<CityDto>> GetCitiesAsync()
    {
        return await _context.Cities
            .Select(d => new CityDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AllergyDto>> GetAllergiesAsync()
    {
        return await _context.Allergies
            .Select(d => new AllergyDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync();
    }
}