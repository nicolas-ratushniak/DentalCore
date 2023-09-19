using DentalCore.Domain.Dto;
using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface IPatientService
{
    public Patient Get(int id);
    public IEnumerable<Patient> GetAll();
    public void Add(PatientCreateDto dto);
    public void Update(PatientUpdateDto dto);
    public int GetDebt(int id);
    public void PayWholeDebt(int id);
    public IEnumerable<Allergy> GetAllergies(int id);
    public IEnumerable<Disease> GetDiseases(int id);
}