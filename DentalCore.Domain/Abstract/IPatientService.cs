using DentalCore.Data.Models;
using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface IPatientService
{
    public Task<PatientRichDto> GetAsync(int id);
    public Task<IEnumerable<PatientDto>> GetAllAsync();
    public Task<IEnumerable<PatientDto>> GetAllIncludeSoftDeletedAsync();
    public Task<int> AddAsync(PatientCreateDto dto);
    public Task UpdateAsync(PatientUpdateDto dto);
    public Task SoftDeleteAsync(int id);
    public Task<IEnumerable<Allergy>> GetAllergiesAsync(int id);
    public Task<IEnumerable<Disease>> GetDiseasesAsync(int id);
    public Task<IEnumerable<Phone>> GetPhonesAsync(int id);
}