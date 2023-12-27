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

    public async Task<VisitRichDto> GetAsync(int id)
    {
        return await _context.Visits
                   .Include(v => v.Patient)
                   .Include(v => v.Doctor)
                   .Include(v => v.Payments)
                   .Select(v => new VisitRichDto
                   {
                       Id = v.Id,
                       VisitDate = v.VisitDate,
                       TotalPrice = v.TotalPrice,
                       AlreadyPayed = v.Payments.Sum(p => p.Sum),
                       Diagnosis = v.Diagnosis,
                       Patient = new PatientDto
                       {
                           Id = v.PatientId,
                           Name = v.Patient.Name,
                           Surname = v.Patient.Surname,
                           Patronymic = v.Patient.Patronymic
                       },
                       Doctor = new UserDto
                       {
                           Id = v.DoctorId,
                           Role = v.Doctor.Role,
                           Login = v.Doctor.Login,
                           Name = v.Doctor.Name,
                           Surname = v.Doctor.Surname,
                           Phone = v.Doctor.Phone
                       }
                   })
                   .SingleOrDefaultAsync(v => v.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<VisitDto>> GetAllAsync()
    {
        return await _context.Visits
            .Select(v => new VisitDto
            {
                Id = v.Id,
                PatientId = v.PatientId,
                DoctorId = v.DoctorId,
                VisitDate = v.VisitDate,
                TotalPrice = v.TotalPrice,
                Diagnosis = v.Diagnosis
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<VisitRichDto>> GetAllRichAsync(DateTime from, DateTime to)
    {
        if (from > to)
        {
            throw new ArgumentException("From should not be greater than To", nameof(from));
        }        
        
        return await _context.Visits
            .Include(v => v.Patient)
            .Include(v => v.Doctor)
            .Include(v => v.Payments)
            .Where(v =>
                v.VisitDate >= from &&
                v.VisitDate <= to)
            .Select(v => new VisitRichDto
            {
                Id = v.Id,
                VisitDate = v.VisitDate,
                TotalPrice = v.TotalPrice,
                AlreadyPayed = v.Payments.Sum(p => p.Sum),
                Diagnosis = v.Diagnosis,
                Patient = new PatientDto
                {
                    Id = v.PatientId,
                    Name = v.Patient.Name,
                    Surname = v.Patient.Surname,
                    Patronymic = v.Patient.Patronymic
                },
                Doctor = new UserDto
                {
                    Id = v.DoctorId,
                    Role = v.Doctor.Role,
                    Login = v.Doctor.Login,
                    Name = v.Doctor.Name,
                    Surname = v.Doctor.Surname,
                    Phone = v.Doctor.Phone
                }
            })
            .ToListAsync();
    }

    public async Task<int> AddAsync(VisitCreateDto dto)
    {
        var createdOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (!dto.TreatmentItems.Any())
        {
            throw new ValidationException("Не обрано жодної процедури");
        }

        var itemsNoDuplicates = dto.TreatmentItems
            .Where(t => t.Quantity > 0)
            .GroupBy(t => t.ProcedureId)
            .Select(g => new TreatmentItemCreateDto
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
            VisitDate = dto.Date,
            CreatedOn = createdOn,
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
}