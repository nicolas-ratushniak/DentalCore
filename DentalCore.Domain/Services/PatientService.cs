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
        return _context.Patients
            .Where(p => !p.IsDeleted)
            .SingleOrDefault(p => p.Id == id) ?? throw new EntityNotFoundException();
    }

    public Patient GetIncludeSoftDeleted(int id)
    {
        return _context.Patients.Find(id)
            ?? throw new EntityNotFoundException();
    }

    public IEnumerable<Patient> GetAll()
    {
        return _context.Patients.Where(p => !p.IsDeleted);
    }

    public IEnumerable<Patient> GetAllIncludeSoftDeleted()
    {
        return _context.Patients.ToList();
    }

    public int Add(PatientCreateDto dto)
    {
        var createdOn = DateTime.Now;
        
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
            City = city,
            Gender = dto.Gender,
            Name = dto.Name,
            Surname = dto.Surname,
            Patronymic = dto.Patronymic,
            BirthDate = dto.BirthDate,
            CreatedOn = createdOn
        };

        foreach (var phoneDto in dto.Phones)
        {
            var phone = new Phone
            {
                Patient = patient,
                PhoneNumber = phoneDto.PhoneNumber,
                IsMain = phoneDto.IsMain,
                Tag = phoneDto.Tag
            };
            
            patient.Phones.Add(phone);
        }
        
        foreach (var allergyId in dto.AllergyIds)
        {
            var allergy = _context.Allergies.Find(allergyId)
                          ?? throw new EntityNotFoundException("Allergy not found");

            patient.Allergies.Add(allergy);
        }

        foreach (var diseaseId in dto.DiseaseIds)
        {
            var disease = _context.Diseases.Find(diseaseId)
                          ?? throw new EntityNotFoundException("Disease not found");

            patient.Diseases.Add(disease);
        }

        _context.Patients.Add(patient);
        _context.SaveChanges();

        return patient.Id;
    }

    public void Update(PatientUpdateDto dto)
    {
        var updatedOn = DateTime.Now;
        
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
        patient.BirthDate = dto.BirthDate;
        patient.UpdatedOn = updatedOn;

        patient.Phones = new List<Phone>();
        
        foreach (var phoneDto in dto.Phones)
        {
            Validator.ValidateObject(phoneDto, new ValidationContext(phoneDto), true);
            
            var phone = new Phone
            {
                Patient = patient,
                PhoneNumber = phoneDto.PhoneNumber,
                IsMain = phoneDto.IsMain,
                Tag = phoneDto.Tag
            };
            
            patient.Phones.Add(phone);
        }

        patient.Allergies = new List<Allergy>();

        foreach (var allergyId in dto.AllergyIds)
        {
            var allergy = _context.Allergies.Find(allergyId)
                          ?? throw new EntityNotFoundException("Allergy not found");

            patient.Allergies.Add(allergy);
        }

        patient.Diseases = new List<Disease>();

        foreach (var diseaseId in dto.DiseaseIds)
        {
            var disease = _context.Diseases.Find(diseaseId)
                          ?? throw new EntityNotFoundException("Disease not found");

            patient.Diseases.Add(disease);
        }

        _context.Patients.Update(patient);
        _context.SaveChanges();
    }

    public void SoftDelete(int id)
    {
        var deletedOn = DateTime.Now;
        var patient = Get(id);
        
        patient.IsDeleted = true;
        patient.DeletedOn = deletedOn;
        
        _context.Update(patient);
        _context.SaveChanges();
    }

    public IEnumerable<Allergy> GetAllergies(int id)
    {
        var patient = _context.Patients
                          .Include(p => p.Allergies)
                          .SingleOrDefault(p => p.Id == id)
                      ?? throw new EntityNotFoundException();

        return patient.Allergies;
    }

    public IEnumerable<Disease> GetDiseases(int id)
    {
        var patient = _context.Patients
                          .Include(p => p.Diseases)
                          .SingleOrDefault(p => p.Id == id)
                      ?? throw new EntityNotFoundException();

        return patient.Diseases;
    }

    public IEnumerable<Phone> GetPhones(int id)
    {
        return _context.Phones.Where(p => p.PatientId == id);
    }
}