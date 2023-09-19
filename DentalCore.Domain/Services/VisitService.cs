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

        var totalPrice = CalculateTotalPrice(
            dto.TreatmentItems.Sum(t => t.Price * t.Quantity),
            dto.DiscountPercent);

        var firstPayment = dto.FirstPayment;

        if (firstPayment > totalPrice)
        {
            throw new ValidationException("Введена сума перевищує потрібну");
        }

        var visit = new Visit
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            DiscountPercent = dto.DiscountPercent,
            TotalPrice = totalPrice,
            Diagnosis = dto.Diagnosis,
            Date = dto.Date
        };

        _context.Visits.Add(visit);

        if (firstPayment > 0)
        {
            AddPaymentUnsafe(visit.Id, dto.FirstPayment);
        }

        foreach (var treatmentItem in dto.TreatmentItems)
        {
            _context.TreatmentItems.Add(new TreatmentItem
            {
                VisitId = visit.Id,
                ProcedureId = treatmentItem.ProcedureId,
                Quantity = treatmentItem.Quantity,
                Price = treatmentItem.Price,
                IsDiscountValid = treatmentItem.IsDiscountValid
            });
        }

        _context.SaveChanges();
    }

    public void AddPayment(int id, int sum)
    {
        var visit = Get(id);

        if (sum < 0 || sum > visit.TotalPrice - GetDebt(id))
        {
            throw new ArgumentOutOfRangeException(nameof(sum));
        }

        AddPaymentUnsafe(id, sum);
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
        var purePrice = (int)((double)sum / 100 * (100 - discountPercent));
        var rest = purePrice % 50;

        return rest < 25 ? purePrice - rest : purePrice + (50 - rest);
    }

    public IEnumerable<TreatmentItem> GetTreatmentItems(int id)
    {
        return _context.TreatmentItems
            .Where(t => t.VisitId == id);
    }

    private void AddPaymentUnsafe(int id, int sum)
    {
        var payment = new Payment
        {
            VisitId = id,
            DateTime = DateTime.Now,
            Sum = sum
        };

        _context.Payments.Add(payment);
        _context.SaveChanges();
    }
}