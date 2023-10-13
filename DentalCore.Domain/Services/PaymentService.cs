using DentalCore.Data;
using DentalCore.Data.Models;
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

    public async Task<Payment> GetAsync(int id)
    {
        return await _context.Payments.FindAsync(id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<Payment>> GetAllAsync(DateTime from, DateTime to)
    {
        return await _context.Payments
            .Where(p => 
                p.CreatedOn >= from && 
                p.CreatedOn <= to)
            .ToListAsync();
    }

    public async Task<int> GetPatientDebtAsync(int patientId)
    {
        if (!await _context.Patients.AnyAsync(p => p.Id == patientId))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var shouldHavePayed = (await GetPatientVisitsAsync(patientId)).Sum(v => v.TotalPrice);
        var actuallyPayed = (await GetPatientPaymentsAsync(patientId)).Sum(p => p.Sum);

        return shouldHavePayed - actuallyPayed;
    }

    public async Task<int> GetVisitDebtAsync(int visitId)
    {
        var visit = await _context.Visits.FindAsync(visitId)
                    ?? throw new EntityNotFoundException("Visit not found");

        return visit.TotalPrice - await GetMoneyPayedForVisitUnsafeAsync(visitId);
    }

    public async Task PayPatientDebtAsync(int patientId)
    {
        if (!await _context.Patients.AnyAsync(p => p.Id == patientId))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var paymentTime = DateTime.Now;

        var alreadyPayedPerVisit = await _context.Payments
            .Include(p => p.Visit)
            .Where(p => p.Visit.PatientId == patientId)
            .GroupBy(p => p.VisitId)
            .Select(g => new
            {
                VisitId = g.Key,
                AlreadyPaid = g.Sum(payment => payment.Sum)
            })
            .ToListAsync();

        foreach (var visit in await GetPatientVisitsAsync(patientId))
        {
            var alreadyPaid = alreadyPayedPerVisit
                .Single(t => t.VisitId == visit.Id)
                .AlreadyPaid;

            var remainsToPay = visit.TotalPrice - alreadyPaid;

            if (remainsToPay <= 0)
            {
                continue;
            }

            var payment = new Payment
            {
                VisitId = visit.Id,
                CreatedOn = paymentTime,
                Sum = remainsToPay
            };

            await _context.Payments.AddAsync(payment);
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddVisitPaymentAsync(int visitId, int sum)
    {
        var visit = await _context.Visits.FindAsync(visitId)
                    ?? throw new EntityNotFoundException("Visit not found");

        if (sum < 0 || sum > visit.TotalPrice - await GetMoneyPayedForVisitUnsafeAsync(visitId))
        {
            throw new ArgumentOutOfRangeException(nameof(sum));
        }

        var payment = new Payment
        {
            VisitId = visitId,
            CreatedOn = DateTime.Now,
            Sum = sum,
            Visit = visit
        };

        await _context.Payments.AddAsync(payment);
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
    public async Task<(int, int)> CalculateTotalWithDiscountAsync(IEnumerable<TreatmentItemDto> selectedTreatmentItems, int discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
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
            throw new ArgumentException("The list of items should not have duplicates", nameof(selectedTreatmentItems));
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

        int roundedTotalNoDiscount;
        int roundedTotalWithDiscount;

        if (preciseTotalWithDiscount <= 50)
        {
            roundedTotalNoDiscount = (int)Math.Round(preciseTotalNoDiscount, MidpointRounding.ToPositiveInfinity);
            roundedTotalWithDiscount = (int)Math.Round(preciseTotalWithDiscount, MidpointRounding.ToPositiveInfinity);
        }
        else
        {
            roundedTotalNoDiscount = RoundPrice(preciseTotalNoDiscount);
            roundedTotalWithDiscount = RoundPrice(preciseTotalWithDiscount);
        }

        var discountSum = roundedTotalNoDiscount - roundedTotalWithDiscount;
        return (roundedTotalWithDiscount, discountSum);

        static int RoundPrice(decimal price)
        {
            var remainder = price % 50;
            return remainder < 25 ? (int)(price - remainder) : (int)(price - remainder) + 50;
        }
    }

    private async Task<IEnumerable<Visit>> GetPatientVisitsAsync(int patientId)
    {
        return await _context.Visits
            .Where(v => v.PatientId == patientId)
            .ToListAsync();
    }

    private async Task<IEnumerable<Payment>> GetPatientPaymentsAsync(int patientId)
    {
        return await _context.Payments
            .Include(p => p.Visit)
            .Where(p => p.Visit.PatientId == patientId)
            .ToListAsync();
    }

    private async Task<int> GetMoneyPayedForVisitUnsafeAsync(int visitId)
    {
        return await _context.Payments
            .Where(p => p.VisitId == visitId)
            .SumAsync(p => p.Sum);
    }
}