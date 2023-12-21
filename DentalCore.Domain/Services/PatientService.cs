using System.ComponentModel.DataAnnotations;
using DentalCore.Data;
using DentalCore.Domain.Dto;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
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

    public async Task<PatientRichDto> GetAsync(int id)
    {
        return await _context.Patients
                   .Include(p => p.City)
                   .Where(p => !p.IsDeleted)
                   .Select(p => new PatientRichDto
                   {
                       Id = p.Id,
                       Gender = p.Gender,
                       Name = p.Name,
                       Surname = p.Surname,
                       Patronymic = p.Patronymic,
                       BirthDate = p.BirthDate,
                       City = new CityDto
                       {
                           Id = p.CityId,
                           Name = p.City.Name
                       }
                   })
                   .SingleOrDefaultAsync(p => p.Id == id)
               ?? throw new EntityNotFoundException();
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        return await _context.Patients
            .Where(p => !p.IsDeleted)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                Name = p.Name,
                Surname = p.Surname,
                Patronymic = p.Patronymic
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PatientDto>> GetAllIncludeSoftDeletedAsync()
    {
        return await _context.Patients
            .Select(p => new PatientDto
            {
                Id = p.Id,
                Name = p.Name,
                Surname = p.Surname,
                Patronymic = p.Patronymic
            })
            .ToListAsync();
    }

    public async Task<int> AddAsync(PatientCreateDto dto)
    {
        var createdOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (await _context.Patients.AnyAsync(p =>
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

        var city = await _context.Cities.FindAsync(dto.CityId)
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

        var allAllergies = await _context.Allergies.ToListAsync();

        foreach (var allergyId in dto.AllergyIds)
        {
            var allergy = allAllergies.SingleOrDefault(a => a.Id == allergyId)
                          ?? throw new EntityNotFoundException("Allergy not found");

            patient.Allergies.Add(allergy);
        }

        var allDiseases = await _context.Diseases.ToListAsync();

        foreach (var diseaseId in dto.DiseaseIds)
        {
            var disease = allDiseases.SingleOrDefault(d => d.Id == diseaseId)
                          ?? throw new EntityNotFoundException("Disease not found");

            patient.Diseases.Add(disease);
        }

        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();

        return patient.Id;
    }

    public async Task UpdateAsync(PatientUpdateDto dto)
    {
        var updatedOn = DateTime.Now;

        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        if (await _context.Patients.AnyAsync(p =>
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

        var city = await _context.Cities.FindAsync(dto.CityId)
                   ?? throw new EntityNotFoundException("City not found");

        var patient = await GetEntityAsync(dto.Id);

        patient.CityId = dto.CityId;
        patient.City = city;
        patient.Gender = dto.Gender;
        patient.Name = dto.Name;
        patient.Surname = dto.Surname;
        patient.Patronymic = dto.Patronymic;
        patient.BirthDate = dto.BirthDate;
        patient.UpdatedOn = updatedOn;
        patient.Phones = GetUpdatedPhones(patient, patient.Phones, dto.Phones);
        patient.Allergies = await GetUpdatedAllergiesAsync(patient.Allergies, dto.AllergyIds);
        patient.Diseases = await GetUpdatedDiseasesAsync(patient.Diseases, dto.DiseaseIds);

        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var deletedOn = DateTime.Now;
        var patient = await GetEntityAsync(id);

        patient.IsDeleted = true;
        patient.DeletedOn = deletedOn;

        _context.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Allergy>> GetAllergiesAsync(int id)
    {
        var patient = await _context.Patients
                          .Include(p => p.Allergies)
                          .SingleOrDefaultAsync(p => p.Id == id)
                      ?? throw new EntityNotFoundException();

        return patient.Allergies;
    }

    public async Task<IEnumerable<Disease>> GetDiseasesAsync(int id)
    {
        var patient = await _context.Patients
                          .Include(p => p.Diseases)
                          .SingleOrDefaultAsync(p => p.Id == id)
                      ?? throw new EntityNotFoundException();

        return patient.Diseases;
    }

    public async Task<IEnumerable<Phone>> GetPhonesAsync(int id)
    {
        return await _context.Phones
            .Where(p => p.PatientId == id)
            .ToListAsync();
    }

    private async Task<Patient> GetEntityAsync(int id)
    {
        return await _context.Patients
                   .SingleOrDefaultAsync(p =>
                       p.Id == id &&
                       !p.IsDeleted)
               ?? throw new EntityNotFoundException();
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

    private async Task<List<Allergy>> GetUpdatedAllergiesAsync(ICollection<Allergy> oldAllergies,
        IEnumerable<int> newAllergyIds)
    {
        var result = oldAllergies
            .Where(a => newAllergyIds.Any(id => id == a.Id))
            .ToList();

        var allAllergies = await _context.Allergies.ToListAsync();
        var onlyNewAllergyIds = newAllergyIds
            .Where(id => oldAllergies.All(a => a.Id != id));

        foreach (var allergyId in onlyNewAllergyIds)
        {
            var allergy = allAllergies.SingleOrDefault(a => a.Id == allergyId)
                          ?? throw new EntityNotFoundException("Allergy not found");

            result.Add(allergy);
        }

        return result;
    }

    private async Task<List<Disease>> GetUpdatedDiseasesAsync(ICollection<Disease> oldDiseases,
        IEnumerable<int> newDiseasesIds)
    {
        var result = oldDiseases
            .Where(d => newDiseasesIds.Any(id => id == d.Id))
            .ToList();

        var allDiseases = await _context.Diseases.ToListAsync();
        var onlyNewDiseaseIds = newDiseasesIds
            .Where(id => oldDiseases.All(d => d.Id != id));

        foreach (var diseaseId in onlyNewDiseaseIds)
        {
            var disease = allDiseases.SingleOrDefault(d => d.Id == diseaseId)
                          ?? throw new EntityNotFoundException("Disease not found");

            result.Add(disease);
        }

        return result;
    }
}