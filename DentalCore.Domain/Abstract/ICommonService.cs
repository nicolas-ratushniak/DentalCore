using DentalCore.Data.Models;

namespace DentalCore.Domain.Abstract;

public interface ICommonService
{
    public Task<IEnumerable<Disease>> GetDiseasesAsync();
    public Task<IEnumerable<City>> GetCitiesAsync();
    public Task<IEnumerable<Allergy>> GetAllergiesAsync();
}