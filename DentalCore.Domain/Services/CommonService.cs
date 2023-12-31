using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Data.Models;
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

    public async Task<AllergyDto> AddAllergyAsync(AllergyCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Allergies.Any(c => c.Name == dto.Name))
        {
            throw new ValidationException("Алергія з такою назвою вже існує");
        }

        var allergy = new Allergy()
        {
            Name = dto.Name
        };

        await _context.Allergies.AddAsync(allergy);
        await _context.SaveChangesAsync();

        return new AllergyDto()
        {
            Id = allergy.Id,
            Name = allergy.Name
        };
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

    public async Task<CityDto> AddCityAsync(CityCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Cities.Any(c => c.Name == dto.Name))
        {
            throw new ValidationException("Місто з такою назвою вже існує");
        }

        var city = new City
        {
            Name = dto.Name
        };

        await _context.Cities.AddAsync(city);
        await _context.SaveChangesAsync();

        return new CityDto
        {
            Id = city.Id,
            Name = city.Name
        };
    }
}