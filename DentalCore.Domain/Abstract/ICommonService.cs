using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface ICommonService
{
    public Task<IEnumerable<DiseaseDto>> GetDiseasesAsync();
    public Task<IEnumerable<CityDto>> GetCitiesAsync();
    public Task<IEnumerable<AllergyDto>> GetAllergiesAsync();
}