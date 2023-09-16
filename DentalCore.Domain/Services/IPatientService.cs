using DentalCore.Domain.Dto;
using DentalCore.Domain.Models;

namespace DentalCore.Domain.Services;

public interface IPatientService
{
    public Patient Get(int id);
    public IEnumerable<Patient> GetAll();
    public void Add(PatientCreateDto dto);
    public void Update(PatientUpdateDto dto);
    public int GetDebt(int id);
    public IEnumerable<Allergy> GetAllergies(int id);
    public IEnumerable<Disease> GetDiseases(int id);
    public IEnumerable<Payment> GetPayments(int id);
}