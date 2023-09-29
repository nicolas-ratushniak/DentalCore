using System.Collections.Generic;
using DentalCore.Data.Models;

namespace DentalCore.Wpf.ViewModels;

public class PatientCreateViewModel : BaseViewModel
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public Gender Gender { get; set; }
    public string Phone { get; set; }
    public int CityId { get; set; }
    public string AllergyNames { get; set; }
    public IEnumerable<int> DiseaseIds { get; set; }
}