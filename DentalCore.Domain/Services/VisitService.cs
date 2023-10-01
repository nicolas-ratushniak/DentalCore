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

        var totalPrice = CalculateTotalPrice(itemsNoDuplicates, dto.DiscountPercent);

        if (dto.FirstPayment > totalPrice)
        {
            throw new ValidationException("Введена сума не має перевищувати суму чеку");
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
            DiscountPercent = dto.DiscountPercent,
            Diagnosis = dto.Diagnosis,
            TotalPrice = totalPrice,
            Date = dto.Date,
            Payments = new List<Payment>(),
            TreatmentItems = new List<TreatmentItem>()
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
                DateTime = DateTime.Now,
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
            DateTime = DateTime.Now,
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

    public int CalculateTotalPrice(IEnumerable<TreatmentItemDto> treatmentItems, int discountPercent)
    {
        if (discountPercent < 0 || discountPercent > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountPercent));
        }
        
        var purePrice = 0.0M;
        var items = treatmentItems.ToList();

        var hasDuplicates = items
            .GroupBy(t => t.ProcedureId)
            .Any(g => g.Count() > 1);

        if (hasDuplicates)
        {
            throw new ArgumentException("The list of items should not have duplicates", nameof(treatmentItems));
        }

        var procedures = _context.Procedures.ToList();
        
        foreach (var item in items)
        {
            var procedure = procedures.Single(p => p.Id == item.ProcedureId);
            
            var priceNoDiscount = procedure.Price * item.Quantity;

            purePrice += procedure.IsDiscountAllowed
                ? priceNoDiscount * (1 - discountPercent / 100.0M)
                : priceNoDiscount;
        }

        if (purePrice <= 50)
        {
            return (int)purePrice;
        }

        var remainder = purePrice % 50;

        return remainder < 25 ? (int)(purePrice - remainder) : (int)(purePrice - remainder) + 50;
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