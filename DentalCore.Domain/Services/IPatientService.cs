using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IPatientService
{
    public Patient Get(int id);
    public Patient GetIncludeSoftDeleted(int id);
    public IEnumerable<Patient> GetAll();
    public IEnumerable<Patient> GetAllIncludeSoftDeleted();
    public int Add(PatientCreateDto dto);
    public void Update(PatientUpdateDto dto);
    public void SoftDelete(int id);
    public int GetDebt(int id);
    public void PayWholeDebt(int id);
    public IEnumerable<Allergy> GetAllergies(int id);
    public IEnumerable<Disease> GetDiseases(int id);
    public IEnumerable<Phone> GetPhones(int id);
}