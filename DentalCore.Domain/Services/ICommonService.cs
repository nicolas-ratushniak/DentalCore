using DentalCore.Data.Models;

namespace DentalCore.Domain.Services;

public interface ICommonService
{
    public IEnumerable<Allergy> GetAllergies();
    public IEnumerable<Disease> GetDiseases();
    public IEnumerable<Payment> GetPayments();
    public IEnumerable<Procedure> GetProcedures();
    public IEnumerable<City> GetCities();
}