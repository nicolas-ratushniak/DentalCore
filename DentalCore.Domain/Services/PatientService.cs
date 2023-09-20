using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DentalCore.Domain.Services;

public class PatientService : IPatientService
{
    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public Patient Get(int id)
    {
        return _context.Patients.Find(id)
               ?? throw new EntityNotFoundException();
    }

    public IEnumerable<Patient> GetAll()
    {
        return _context.Patients.ToList();
    }

    public void Add(PatientCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Patients.Any(p =>
                p.Name == dto.Name && p.Surname == dto.Surname && p.Patronymic == dto.Patronymic))
        {
            throw new ValidationException("У базі вже є пацієнт з таким ПІБ");
        }

        var city = _context.Cities.Find(dto.CityId)
                   ?? throw new EntityNotFoundException("City not found");

        var patient = new Patient
        {
            CityId = dto.CityId,
            Gender = dto.IsMale ? Gender.Male : Gender.Female,
            Name = dto.Name,
            Surname = dto.Surname,
            Patronymic = dto.Patronymic,
            Phone = dto.Phone,
            BirthDate = dto.BirthDate,
            DateCreated = DateTime.Today,
            City = city
        };

        _context.Patients.Add(patient);
        _context.SaveChanges();
    }

    public void Update(PatientUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Patients.Any(p =>
                p.Name == dto.Name && p.Surname == dto.Surname && p.Patronymic == dto.Patronymic && p.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є пацієнт з таким ПІБ");
        }
        
        var city = _context.Cities.Find(dto.CityId)
                   ?? throw new EntityNotFoundException("City not found");
        
        var patient = Get(dto.Id);
        
        patient.CityId = dto.CityId;
        patient.City = city;
        patient.Gender = dto.IsMale ? Gender.Male : Gender.Female;
        patient.Name = dto.Name;
        patient.Surname = dto.Surname;
        patient.Patronymic = dto.Patronymic;
        patient.Phone = dto.Phone;
        patient.BirthDate = dto.BirthDate;

        _context.Patients.Update(patient);
        _context.SaveChanges();
    }

    public int GetDebt(int id)
    {
        var shouldHavePayed = GetVisits(id).Sum(v => v.TotalPrice);
        var actuallyPayed = GetPayments(id).Sum(p => p.Sum);

        return shouldHavePayed - actuallyPayed;
    }

    public void PayWholeDebt(int id)
    {
        var paymentTime = DateTime.Now;
        
        foreach (var visit in GetVisits(id))
        {
            var alreadyPaid = _context.Payments.Where(p => p.VisitId == visit.Id)
                .Sum(p => p.Sum);

            var payment = new Payment
            {
                VisitId = visit.Id,
                DateTime = paymentTime,
                Sum = visit.TotalPrice - alreadyPaid,
            };

            _context.Payments.Add(payment);
        }

        _context.SaveChanges();
    }

    public IEnumerable<Allergy> GetAllergies(int id)
    {
        return _context.Allergies.Where(a => a.PatientId == id);
    }

    public IEnumerable<Disease> GetDiseases(int id)
    {
        var patient = _context.Patients
            .Include(p => p.Diseases)
            .SingleOrDefault(p => p.Id == id)
            ?? throw new EntityNotFoundException();

        return patient.Diseases ?? new List<Disease>();
    }

    private IEnumerable<Visit> GetVisits(int id)
    {
        return _context.Visits.Where(v => v.PatientId == id);
    }
    
    private IEnumerable<Payment> GetPayments(int id)
    {
        return _context.Payments
            .Include(p => p.Visit)
            .Where(p => p.Visit.PatientId == id);
    }
}