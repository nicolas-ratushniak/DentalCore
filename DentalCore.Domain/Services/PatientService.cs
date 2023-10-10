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
                p.Name == dto.Name &&
                p.Surname == dto.Surname &&
                p.Patronymic == dto.Patronymic))
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

        var allAllergies = _context.Allergies.ToList();

        foreach (var allergyId in dto.AllergyIds)
        {
            var allergy = allAllergies.SingleOrDefault(a => a.Id == allergyId)
                          ?? throw new EntityNotFoundException("Allergy not found");

            patient.Allergies.Add(allergy);
        }

        var allDiseases = _context.Diseases.ToList();

        foreach (var diseaseId in dto.DiseaseIds)
        {
            var disease = allDiseases.SingleOrDefault(d => d.Id == diseaseId)
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
                p.Name == dto.Name &&
                p.Surname == dto.Surname
                && p.Patronymic == dto.Patronymic
                && p.Id != dto.Id))
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
        patient.Phones = GetUpdatedPhones(patient, patient.Phones, dto.Phones);
        patient.Allergies = GetUpdatedAllergies(patient.Allergies, dto.AllergyIds);
        patient.Diseases = GetUpdatedDiseases(patient.Diseases, dto.DiseaseIds);

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

    private List<Phone> GetUpdatedPhones(Patient patient, ICollection<Phone> oldPhones,
        IEnumerable<PhoneCreateDto> newPhones)
    {
        var result = oldPhones
            .Where(p => newPhones.Any(phone => phone.PhoneNumber == p.PhoneNumber))
            .ToList();

        foreach (var phoneDto in newPhones)
        {
            Validator.ValidateObject(phoneDto, new ValidationContext(phoneDto), true);

            var phone = oldPhones
                .SingleOrDefault(p => p.PhoneNumber == phoneDto.PhoneNumber);

            if (phone != null)
            {
                phone.IsMain = phoneDto.IsMain;
                phone.Tag = phoneDto.Tag;
            }
            else
            {
                result.Add(new Phone
                {
                    Patient = patient,
                    PhoneNumber = phoneDto.PhoneNumber,
                    IsMain = phoneDto.IsMain,
                    Tag = phoneDto.Tag
                });
            }
        }

        return result;
    }

    private List<Allergy> GetUpdatedAllergies(IEnumerable<Allergy> oldAllergies, IEnumerable<int> newAllergyIds)
    {
        var result = oldAllergies
            .Where(a => newAllergyIds.Any(id => id == a.Id))
            .ToList();

        var allAllergies = _context.Allergies.ToList();

        foreach (var allergyId in newAllergyIds
                     .Where(id => allAllergies.All(a => a.Id != id)))
        {
            var allergy = allAllergies.SingleOrDefault(a => a.Id == allergyId)
                          ?? throw new EntityNotFoundException("Allergy not found");

            result.Add(allergy);
        }

        return result;
    }

    private List<Disease> GetUpdatedDiseases(IEnumerable<Disease> oldDiseases, IEnumerable<int> newDiseasesIds)
    {
        var result = oldDiseases
            .Where(d => newDiseasesIds.Any(id => id == d.Id))
            .ToList();

        var allDiseases = _context.Diseases.ToList();

        foreach (var diseaseId in newDiseasesIds
                     .Where(id => allDiseases.All(d => d.Id != id)))
        {
            var disease = allDiseases.SingleOrDefault(d => d.Id == diseaseId)
                          ?? throw new EntityNotFoundException("Disease not found");

            result.Add(disease);
        }

        return result;
    }
}