using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class VisitService : IVisitService
{
    private readonly AppDbContext _context;
    private readonly IPaymentService _paymentService;

    public VisitService(AppDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<Visit> GetAsync(int id)
    {
        return await _context.Visits.FindAsync(id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<Visit>> GetAllAsync()
    {
        return await _context.Visits.ToListAsync();
    }

    public async Task<int> AddAsync(VisitCreateDto dto)
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

        var (totalPrice, discountSum) =
            await _paymentService.CalculateTotalWithDiscountAsync(itemsNoDuplicates, dto.DiscountPercent);

        if (dto.FirstPayment > totalPrice)
        {
            throw new ValidationException("Внесена сума не має перевищувати суму чеку");
        }

        var patient = await _context.Patients.FindAsync(dto.PatientId)
                      ?? throw new EntityNotFoundException("Patient not found");

        var doctor = await _context.Users
                         .Where(u => u.Role == UserRole.Doctor && !u.IsDeleted)
                         .SingleOrDefaultAsync(d => d.Id == dto.DoctorId)
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

        var procedures = await _context.Procedures.ToListAsync();

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

            visit.Payments = new List<Payment> { payment };
        }

        await _context.Visits.AddAsync(visit);
        await _context.SaveChangesAsync();

        return visit.Id;
    }

    public async Task<IEnumerable<TreatmentItem>> GetTreatmentItemsAsync(int id)
    {
        return await _context.TreatmentItems
            .Where(t => t.VisitId == id)
            .ToListAsync();
    }
}