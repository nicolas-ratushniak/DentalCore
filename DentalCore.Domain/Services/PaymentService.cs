using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class PaymentService : IPaymentService
{
    private readonly AppDbContext _context;

    public PaymentService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentDto> GetAsync(int id)
    {
        return await _context.Payments
                   .Select(p => new PaymentDto
                   {
                       Id = p.Id,
                       VisitId = p.VisitId,
                       Sum = p.Sum,
                       PaymentDate = p.PaymentDate
                   })
                   .SingleOrDefaultAsync(p => p.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync(DateTime from, DateTime to)
    {
        return await _context.Payments
            .Where(p =>
                p.PaymentDate >= from &&
                p.PaymentDate <= to)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                VisitId = p.VisitId,
                Sum = p.Sum,
                PaymentDate = p.PaymentDate
            })
            .ToListAsync();
    }

    public async Task AddAsync(PaymentCreateDto dto)
    {
        var createdOn = DateTime.Now;

        if (dto.PaymentDate > createdOn)
        {
            throw new ValidationException("Payment from the future is not acceptable");
        }

        if (dto.Sum < 0 || dto.Sum > await GetVisitDebtAsync(dto.VisitId))
        {
            throw new ArgumentOutOfRangeException(nameof(dto));
        }

        var visit = await _context.Visits.FindAsync(dto.VisitId)
                    ?? throw new EntityNotFoundException("Visit not found");

        var payment = new Payment
        {
            VisitId = dto.VisitId,
            PaymentDate = dto.PaymentDate,
            Sum = dto.Sum,
            Visit = visit,
            CreatedOn = createdOn
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetPatientDebtAsync(int patientId)
    {
        if (!await _context.Patients.AnyAsync(p => p.Id == patientId))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var visitsWithPayments = await GetPatientVisitsIncludePaymentsUnsafeAsync(patientId);
        
        var shouldHavePayed = visitsWithPayments.Sum(v => v.TotalPrice);
        
        var actuallyPayed = visitsWithPayments
            .SelectMany(v => v.Payments)
            .Sum(p => p.Sum);

        return shouldHavePayed - actuallyPayed;
    }

    public async Task<int> GetVisitDebtAsync(int visitId)
    {
        var visit = await _context.Visits.FindAsync(visitId)
                    ?? throw new EntityNotFoundException("Visit not found");

        return visit.TotalPrice - await GetMoneyPayedForVisitUnsafeAsync(visitId);
    }

    public async Task PayPatientDebtAsync(int patientId, DateTime paymentDate)
    {
        var createdOn = DateTime.Now;

        if (!await _context.Patients.AnyAsync(p => p.Id == patientId))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var paymentPerVisitWithDebt = (await GetPatientVisitsIncludePaymentsUnsafeAsync(patientId))
            .Select(v => new Payment
            {
                VisitId = v.Id,
                Visit = v,
                PaymentDate = paymentDate,
                CreatedOn = createdOn,
                Sum = v.TotalPrice - v.Payments.Sum(p => p.Sum)
            })
            .Where(p => p.Sum > 0)
            .ToList();

        if (!paymentPerVisitWithDebt.Any())
        {
            throw new InvalidOperationException("A patient has no visits or no debt");
        }

        await _context.Payments.AddRangeAsync(paymentPerVisitWithDebt);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetMoneyPayedForVisitAsync(int visitId)
    {
        if (!await _context.Visits.AnyAsync(v => v.Id == visitId))
        {
            throw new EntityNotFoundException("Visit not found");
        }

        return await GetMoneyPayedForVisitUnsafeAsync(visitId);
    }

    /// <summary>
    /// Returns (total sum, total discount)
    /// </summary>
    public async Task<(int, int)> CalculateTotalWithDiscountAsync(IEnumerable<TreatmentItemDto> selectedTreatmentItems,
        int discountPercent)
    {
        if (discountPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountPercent));
        }

        var preciseTotalWithDiscount = 0.0M;
        var preciseTotalNoDiscount = 0.0M;
        var items = selectedTreatmentItems.ToList();

        var hasDuplicates = items
            .GroupBy(t => t.ProcedureId)
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
        {
            throw new ArgumentException(
                "The list of items should not have duplicates", 
                nameof(selectedTreatmentItems));
        }

        var procedures = await _context.Procedures.ToListAsync();

        foreach (var item in items)
        {
            var procedure = procedures.Single(p => p.Id == item.ProcedureId);

            var itemPriceNoDiscount = procedure.Price * item.Quantity;

            preciseTotalNoDiscount += itemPriceNoDiscount;
            preciseTotalWithDiscount += procedure.IsDiscountAllowed
                ? itemPriceNoDiscount * (1 - discountPercent / 100.0M)
                : itemPriceNoDiscount;
        }

        var roundedTotalNoDiscount = RoundToComfortablePrice(preciseTotalNoDiscount);
        var roundedTotalWithDiscount = RoundToComfortablePrice(preciseTotalWithDiscount);

        var discountSum = roundedTotalNoDiscount - roundedTotalWithDiscount;
        return (roundedTotalWithDiscount, discountSum);

        static int RoundToComfortablePrice(decimal price)
        {
            if (price <= 50)
            {
                return (int)Math.Round(price, MidpointRounding.ToPositiveInfinity);
            }
            
            var remainder = price % 50;
            return remainder < 25 ? (int)(price - remainder) : (int)(price - remainder) + 50;
        }
    }

    private async Task<List<Visit>> GetPatientVisitsIncludePaymentsUnsafeAsync(int patientId)
    {
        return await _context.Visits
            .Include(v => v.Payments)
            .Where(v => v.PatientId == patientId)
            .ToListAsync();
    }

    private async Task<int> GetMoneyPayedForVisitUnsafeAsync(int visitId)
    {
        return await _context.Payments
            .Where(p => p.VisitId == visitId)
            .SumAsync(p => p.Sum);
    }
}