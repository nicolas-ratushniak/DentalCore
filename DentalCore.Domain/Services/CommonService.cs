﻿using DentalCore.Data;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public class CommonService : ICommonService
{
    private readonly AppDbContext _context;

    public CommonService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Allergy> GetAllergies()
    {
        return _context.Allergies.ToList();
    }

    public IEnumerable<Disease> GetDiseases()
    {
        return _context.Diseases.ToList();
    }

    public IEnumerable<Payment> GetPayments()
    {
        return _context.Payments.ToList();
    }

    public IEnumerable<Procedure> GetProcedures()
    {
        return _context.Procedures
            .Where(p => !p.IsDeleted)
            .ToList();
    }

    public IEnumerable<City> GetCities()
    {
        return _context.Cities.ToList();
    }
}