using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;

namespace DentalCore.Domain.Services;

public class VisitService : IVisitService
{
    private readonly AppDbContext _context;

    public VisitService(AppDbContext context)
    {
        _context = context;
    }

    public Visit Get(int id)
    {
        return _context.Visits.Find(id)
               ?? throw new EntityNotFoundException();
    }

    public IEnumerable<Visit> GetAll()
    {
        return _context.Visits.ToList();
    }

    public int Add(VisitCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (!dto.TreatmentItems.Any())
        {
            throw new ValidationException("Не обрано жодної процедури");
        }

        var itemsNoDuplicates = dto.TreatmentItems
            .Where(t => t.Quantity > 0)
            .GroupBy(t => t.ProcedureId)
            .Select(g => new TreatmentItemDto
            {
                ProcedureId = g.Key,
                Quantity = g.Sum(item => item.Quantity)
            })
            .ToList();

        var totalPrice = CalculateTotalWithDiscount(itemsNoDuplicates, dto.DiscountPercent, out int discountSum);

        if (dto.FirstPayment > totalPrice)
        {
            throw new ValidationException("Внесена сума не має перевищувати суму чеку");
        }

        var patient = _context.Patients.Find(dto.PatientId)
                      ?? throw new EntityNotFoundException("Patient not found");

        var doctor = _context.Users
                         .Where(u => u.Role == UserRole.Doctor && !u.IsDeleted)
                         .SingleOrDefault(d => d.Id == dto.DoctorId)
                     ?? throw new EntityNotFoundException("Doctor not found");

        var visit = new Visit
        {
            PatientId = dto.PatientId,
            Patient = patient,
            DoctorId = dto.DoctorId,
            Doctor = doctor,
            DiscountSum = discountSum,
            Diagnosis = dto.Diagnosis,
            TotalPrice = totalPrice,
            CreatedOn = dto.Date,
            Payments = new List<Payment>(),
            TreatmentItems = new List<TreatmentItem>(),
        };

        var procedures = _context.Procedures.ToList();

        foreach (var item in itemsNoDuplicates)
        {
            Validator.ValidateObject(item, new ValidationContext(item), true);

            var procedure = procedures.Single(p => p.Id == item.ProcedureId);

            visit.TreatmentItems.Add(new TreatmentItem
            {
                ProcedureId = item.ProcedureId,
                Quantity = item.Quantity,
                Price = procedure.Price,
                IsDiscountAllowed = procedure.IsDiscountAllowed,
                Visit = visit,
                Procedure = procedure
            });
        }

        if (dto.FirstPayment > 0)
        {
            var payment = new Payment
            {
                CreatedOn = DateTime.Now,
                Sum = dto.FirstPayment,
                Visit = visit
            };

            visit.Payments = new List<Payment>();
            visit.Payments.Add(payment);
        }

        _context.Visits.Add(visit);
        _context.SaveChanges();

        return visit.Id;
    }

    public void AddPayment(int id, int sum)
    {
        var visit = Get(id);

        if (sum < 0 || sum > visit.TotalPrice - GetMoneyPayed(id))
        {
            throw new ArgumentOutOfRangeException(nameof(sum));
        }

        var payment = new Payment
        {
            VisitId = id,
            CreatedOn = DateTime.Now,
            Sum = sum,
            Visit = visit
        };

        _context.Payments.Add(payment);
        _context.SaveChanges();
    }

    public int GetDebt(int id)
    {
        var visit = Get(id);
        return visit.TotalPrice - GetMoneyPayedUnsafe(id);
    }

    public int GetMoneyPayed(int id)
    {
        if (!_context.Visits.Any(v => v.Id == id))
        {
            throw new EntityNotFoundException("Visit not found");
        }

        return GetMoneyPayedUnsafe(id);
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

    public IEnumerable<TreatmentItem> GetTreatmentItems(int id)
    {
        return _context.TreatmentItems
            .Where(t => t.VisitId == id);
    }

    private int GetMoneyPayedUnsafe(int id)
    {
        return _context.Payments
            .Where(p => p.VisitId == id)
            .Sum(p => p.Sum);
    }
}