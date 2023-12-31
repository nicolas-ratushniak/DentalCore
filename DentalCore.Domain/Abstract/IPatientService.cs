using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IPatientService
{
    public Task<PatientDto> GetAsync(int id);
    public Task<PatientRichDto> GetRichAsync(int id);
    public Task<IEnumerable<PatientDto>> GetAllAsync();
    public Task<IEnumerable<PatientDto>> GetAllIncludeSoftDeletedAsync();
    public Task<int> AddAsync(PatientCreateDto dto);
    public Task UpdateAsync(PatientUpdateDto dto);
    public Task SoftDeleteAsync(int id);
    public Task<IEnumerable<AllergyDto>> GetAllergiesAsync(int id);
    public Task<IEnumerable<DiseaseDto>> GetDiseasesAsync(int id);
    public Task<IEnumerable<PhoneDto>> GetPhonesAsync(int id);
}