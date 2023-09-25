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

    public void Add(VisitCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (!dto.TreatmentItems.Any())
        {
            throw new ValidationException("Не обрано жодної процедури");
        }

        var totalPrice = CalculateTotalPrice(dto.TreatmentItems, dto.DiscountPercent);

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

        foreach (var item in dto.TreatmentItems)
        {
            var procedure = FindProcedure(item.ProcedureId);

            visit.TreatmentItems.Add(new TreatmentItem
            {
                ProcedureId = item.ProcedureId,
                Quantity = item.Quantity,
                Price = procedure.Price,
                IsDiscountValid = procedure.IsDiscountValid,
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
    }

    public void AddPayment(int id, int sum)
    {
        var visit = Get(id);

        if (sum < 0 || sum > visit.TotalPrice - GetDebt(id))
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
        return visit.TotalPrice - GetMoneyPayed(id);
    }

    public int GetMoneyPayed(int id)
    {
        return _context.Payments
            .Where(p => p.VisitId == id)
            .Sum(p => p.Sum);
    }

    public int CalculateTotalPrice(IEnumerable<TreatmentItemDto> treatmentItems, int discountPercent)
    {
        var purePrice = treatmentItems
            .Select(item => FindProcedure(item.ProcedureId))
            .Select(procedure => 
                procedure.IsDiscountValid
                ? (int)Math.Round(procedure.Price * (1 - discountPercent / 100.0), MidpointRounding.ToPositiveInfinity)
                : procedure.Price)
            .Sum();

        if (purePrice <= 50)
        {
            return 50;
        }

        var remainder = purePrice % 50;

        return remainder < 25 ? purePrice - remainder : purePrice - remainder + 50;
    }

    public IEnumerable<TreatmentItem> GetTreatmentItems(int id)
    {
        return _context.TreatmentItems
            .Where(t => t.VisitId == id);
    }

    private Procedure FindProcedure(int procedureId)
    {
        return _context.Procedures
                .Where(p => !p.IsDeleted)
                .SingleOrDefault(p => p.Id == procedureId)
               ?? throw new EntityNotFoundException("Procedure not found");
    }
}