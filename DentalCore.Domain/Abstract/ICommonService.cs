using DentalCore.Domain.Dto;

namespace DentalCore.Domain.Abstract;

public interface ICommonService
{
    public Task<IEnumerable<DiseaseDto>> GetDiseasesAsync();
    public Task<IEnumerable<AllergyDto>> GetAllergiesAsync();
    public Task<AllergyDto> AddAllergyAsync(AllergyCreateDto dto);
    public Task<IEnumerable<CityDto>> GetCitiesAsync();
    public Task<CityDto> AddCityAsync(CityCreateDto dto);
}