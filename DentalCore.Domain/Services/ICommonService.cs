using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface ICommonService
{
    public Task<IEnumerable<Disease>> GetDiseasesAsync();
    public Task<IEnumerable<City>> GetCitiesAsync();
    public Task<IEnumerable<Allergy>> GetAllergiesAsync();
}