using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class CommonService : ICommonService
{
    private readonly AppDbContext _context;

    public CommonService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Disease>> GetDiseasesAsync()
    {
        return await _context.Diseases.ToListAsync();
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.ToListAsync();
    }

    public async Task<IEnumerable<Allergy>> GetAllergiesAsync()
    {
        return await _context.Allergies.ToListAsync();
    }
}