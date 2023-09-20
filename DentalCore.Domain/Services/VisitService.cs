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

        var patient = _context.Patients.Find(dto.PatientId)
                      ?? throw new EntityNotFoundException("Patient not found");

        var doctor = _context.Users
                         .Where(u => u.Role == UserRole.Doctor)
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
            Date = dto.Date,
            Payments = new List<Payment>(),
            TreatmentItems = new List<TreatmentItem>()
        };

        foreach (var item in dto.TreatmentItems)
        {
            var procedure = _context.Procedures.Find(item.ProcedureId)
                            ?? throw new EntityNotFoundException("Procedure not found");

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

        visit.TotalPrice = CalculateTotalPrice(
            visit.TreatmentItems.Sum(t => t.Price * t.Quantity),
            dto.DiscountPercent);

        var firstPayment = dto.FirstPayment;

        if (firstPayment > visit.TotalPrice)
        {
            throw new ValidationException("Введена сума перевищує потрібну");
        }

        if (firstPayment > 0)
        {
            var payment = new Payment
            {
                DateTime = DateTime.Now,
                Sum = firstPayment,
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

    public int CalculateTotalPrice(int sum, int discountPercent)
    {
        var purePrice = (int)Math.Round(sum * (1 - discountPercent / 100.0), MidpointRounding.ToPositiveInfinity);
        var remainder = purePrice % 50;

        return remainder < 25 ? purePrice - remainder : purePrice - remainder + 50;
    }

    public IEnumerable<TreatmentItem> GetTreatmentItems(int id)
    {
        return _context.TreatmentItems
            .Where(t => t.VisitId == id);
    }
}