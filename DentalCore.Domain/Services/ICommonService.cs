using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface ICommonService
{
    public IEnumerable<Disease> GetDiseases();
    public IEnumerable<Payment> GetPayments();
    public IEnumerable<City> GetCities();
}