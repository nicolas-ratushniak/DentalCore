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

    public Payment Get(int id)
    {
        return _context.Payments.Find(id)
               ?? throw new EntityNotFoundException();
    }

    public IEnumerable<Payment> GetAll(DateTime from, DateTime to)
    {
        return _context.Payments
            .Where(p => 
                p.CreatedOn >= from && 
                p.CreatedOn <= to);
    }

    public int GetPatientDebt(int patientId)
    {
        if (!_context.Patients.Any(p => p.Id == patientId))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var shouldHavePayed = GetPatientVisits(patientId).Sum(v => v.TotalPrice);
        var actuallyPayed = GetPatientPayments(patientId).Sum(p => p.Sum);

        return shouldHavePayed - actuallyPayed;
    }

    public int GetVisitDebt(int visitId)
    {
        var visit = _context.Visits.Find(visitId)
                    ?? throw new EntityNotFoundException("Visit not found");

        return visit.TotalPrice - GetMoneyPayedForVisitUnsafe(visitId);
    }

    public void PayPatientDebt(int patientId)
    {
        if (!_context.Patients.Any(p => p.Id == patientId))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var paymentTime = DateTime.Now;

        var alreadyPayedPerVisit = _context.Payments
            .Include(p => p.Visit)
            .Where(p => p.Visit.PatientId == patientId)
            .GroupBy(p => p.VisitId)
            .Select(g => new
            {
                VisitId = g.Key,
                AlreadyPaid = g.Sum(payment => payment.Sum)
            })
            .ToList();

        foreach (var visit in GetPatientVisits(patientId))
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

            _context.Payments.Add(payment);
        }

        _context.SaveChanges();
    }

    public void AddVisitPayment(int visitId, int sum)
    {
        var visit = _context.Visits.Find(visitId)
                    ?? throw new EntityNotFoundException("Visit not found");

        if (sum < 0 || sum > visit.TotalPrice - GetMoneyPayedForVisitUnsafe(visitId))
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

        _context.Payments.Add(payment);
        _context.SaveChanges();
    }

    public int GetMoneyPayedForVisit(int visitId)
    {
        if (!_context.Visits.Any(v => v.Id == visitId))
        {
            throw new EntityNotFoundException("Visit not found");
        }

        return GetMoneyPayedForVisitUnsafe(visitId);
    }

    public int CalculateTotalWithDiscount(IEnumerable<TreatmentItemDto> selectedTreatmentItems, int discountPercent,
        out int discountSum)
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

        var procedures = _context.Procedures.ToList();

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

        discountSum = roundedTotalNoDiscount - roundedTotalWithDiscount;
        return roundedTotalWithDiscount;

        static int RoundPrice(decimal price)
        {
            var remainder = price % 50;
            return remainder < 25 ? (int)(price - remainder) : (int)(price - remainder) + 50;
        }
    }

    private IEnumerable<Visit> GetPatientVisits(int patientId)
    {
        return _context.Visits.Where(v => v.PatientId == patientId);
    }

    private IEnumerable<Payment> GetPatientPayments(int patientId)
    {
        return _context.Payments
            .Include(p => p.Visit)
            .Where(p => p.Visit.PatientId == patientId);
    }

    private int GetMoneyPayedForVisitUnsafe(int visitId)
    {
        return _context.Payments
            .Where(p => p.VisitId == visitId)
            .Sum(p => p.Sum);
    }
}