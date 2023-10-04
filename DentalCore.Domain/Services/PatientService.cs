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

    public int Add(PatientCreateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Patients.Any(p =>
                p.Name == dto.Name && p.Surname == dto.Surname && p.Patronymic == dto.Patronymic))
        {
            throw new ValidationException("У базі вже є пацієнт з таким ПІБ");
        }

        if (dto.BirthDate > DateTime.Now)
        {
            throw new ValidationException("Некоректна дата народження. Подорожі у часі заборонені");
        }

        var city = _context.Cities.Find(dto.CityId)
                   ?? throw new EntityNotFoundException("City not found");

        var patient = new Patient
        {
            CityId = dto.CityId,
            Gender = dto.Gender,
            Name = dto.Name,
            Surname = dto.Surname,
            Patronymic = dto.Patronymic,
            Phone = dto.Phone,
            BirthDate = dto.BirthDate,
            DateAdded = DateTime.Today,
            City = city,
            Allergies = new List<Allergy>(),
            Diseases = new List<Disease>()
        };

        if (dto.AllergyNames is not null)
        {
            foreach (var allergyName in dto.AllergyNames)
            {
                var allergy = new Allergy
                {
                    Name = allergyName,
                    Patient = patient
                };

                patient.Allergies.Add(allergy);
            }
        }
        
        if (dto.DiseaseIds is not null)
        {
            foreach (var diseaseId in dto.DiseaseIds)
            {
                var disease = _context.Diseases.Find(diseaseId)
                              ?? throw new EntityNotFoundException("Disease not found");
                
                patient.Diseases.Add(disease);
            }
        }

        _context.Patients.Add(patient);
        _context.SaveChanges();

        return patient.Id;
    }

    public void Update(PatientUpdateDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (_context.Patients.Any(p =>
                p.Name == dto.Name && p.Surname == dto.Surname && p.Patronymic == dto.Patronymic && p.Id != dto.Id))
        {
            throw new ValidationException("У базі вже є пацієнт з таким ПІБ");
        }

        if (dto.BirthDate > DateTime.Now)
        {
            throw new ValidationException("Некоректна дата народження. Подорожі у часі заборонені");
        }

        var city = _context.Cities.Find(dto.CityId)
                   ?? throw new EntityNotFoundException("City not found");

        var patient = Get(dto.Id);

        patient.CityId = dto.CityId;
        patient.City = city;
        patient.Gender = dto.Gender;
        patient.Name = dto.Name;
        patient.Surname = dto.Surname;
        patient.Patronymic = dto.Patronymic;
        patient.Phone = dto.Phone;
        patient.BirthDate = dto.BirthDate;
        
        if (dto.AllergyNames is not null)
        {
            patient.Allergies = new List<Allergy>();
            
            foreach (var allergyName in dto.AllergyNames)
            {
                var allergy = new Allergy
                {
                    Name = allergyName,
                    Patient = patient
                };

                patient.Allergies.Add(allergy);
            }
        }
        else
        {
            patient.Allergies = null;
        }
        
        if (dto.DiseaseIds is not null)
        {
            patient.Diseases = new List<Disease>();
            
            foreach (var diseaseId in dto.DiseaseIds)
            {
                var disease = _context.Diseases.Find(diseaseId)
                              ?? throw new EntityNotFoundException("Disease not found");
                
                patient.Diseases.Add(disease);
            }
        }
        else
        {
            patient.Diseases = null;
        }

        _context.Patients.Update(patient);
        _context.SaveChanges();
    }

    public int GetDebt(int id)
    {
        if (!_context.Patients.Any(p => p.Id == id))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var shouldHavePayed = GetVisits(id).Sum(v => v.TotalPrice);
        var actuallyPayed = GetPayments(id).Sum(p => p.Sum);

        return shouldHavePayed - actuallyPayed;
    }

    public void PayWholeDebt(int id)
    {
        if (!_context.Patients.Any(p => p.Id == id))
        {
            throw new EntityNotFoundException("Patient not found");
        }

        var paymentTime = DateTime.Now;

        foreach (var visit in GetVisits(id))
        {
            var alreadyPaid = _context.Payments.Where(p => p.VisitId == visit.Id)
                .Sum(p => p.Sum);

            var remainsToPay = visit.TotalPrice - alreadyPaid;

            if (remainsToPay <= 0)
            {
                continue;
            }

            var payment = new Payment
            {
                VisitId = visit.Id,
                DateTime = paymentTime,
                Sum = remainsToPay
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