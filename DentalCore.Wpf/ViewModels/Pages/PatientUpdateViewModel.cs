using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Components;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class PatientUpdateViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IPatientService _patientService;
    private readonly ICommonService _commonService;
    private readonly int _patientId;

    private string _name = string.Empty;
    private string _surname = string.Empty;
    private string _patronymic = string.Empty;
    private Gender _gender;
    private string _phone = string.Empty;
    private string _birthDate = string.Empty;
    private string? _errorMessage;
    public ICommand CancelCommand { get; }
    public ICommand SubmitCommand { get; }

    public ObservableCollection<DiseaseListItemViewModel> Diseases { get; }
    public AllergyMultiSelectorViewModel AllergyMultiSelector { get; }
    public CitySelectorViewModel CitySelector { get; }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (value == _errorMessage) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Surname
    {
        get => _surname;
        set
        {
            if (value == _surname) return;
            _surname = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Patronymic
    {
        get => _patronymic;
        set
        {
            if (value == _patronymic) return;
            _patronymic = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public Gender Gender
    {
        get => _gender;
        set
        {
            if (value == _gender) return;
            _gender = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            if (value == _phone) return;
            _phone = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string BirthDate
    {
        get => _birthDate;
        set
        {
            if (value == _birthDate) return;
            _birthDate = value;

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public PatientUpdateViewModel(
        int id,
        INavigationService navigationService,
        IPatientService patientService,
        ICommonService commonService)
    {
        _patientId = id;
        _navigationService = navigationService;
        _patientService = patientService;
        _commonService = commonService;
        Diseases = new ObservableCollection<DiseaseListItemViewModel>();

        AllergyMultiSelector = new AllergyMultiSelectorViewModel();
        CitySelector = new CitySelectorViewModel(AddCityCallback);

        CancelCommand = new RelayCommand(() => _navigationService.NavigateTo(PageType.Patients));
        SubmitCommand = new AsyncRelayCommand(Update_Execute);
    }

    public override async Task LoadDataAsync()
    {
        CitySelector.Cities.Clear();
        
        foreach (var city in await GetCitiesAsync())
        {
            CitySelector.Cities.Add(city);
        }
        
        AllergyMultiSelector.Allergies.Clear();
        
        foreach (var allergy in await GetPatientAllergiesAsync(_patientId))
        {
            AllergyMultiSelector.Allergies.Add(allergy);
        }
        
        Diseases.Clear();

        foreach (var disease in await GetDiseasesAsync())
        {
            Diseases.Add(disease);
        }
        
        var patient = await _patientService.GetRichAsync(_patientId);

        Name = patient.Name;
        Surname = patient.Surname;
        Patronymic = patient.Patronymic;
        BirthDate = patient.BirthDate.ToString("dd.MM.yyyy");
        Gender = patient.Gender;
        Phone = (await _patientService.GetPhonesAsync(patient.Id)).First().PhoneNumber;
        CitySelector.SelectedCity = CitySelector.Cities.Single(c => c.Id == patient.City.Id);
    }
    
    private async Task AddCityCallback(string cityName)
    {
        var dto = new CityCreateDto
        {
            Name = cityName
        };

        try
        {
            var city = await _commonService.AddCityAsync(dto);
            var cityListItem = new CityListItemViewModel
            {
                Id = city.Id,
                Name = city.Name
            };
            
            CitySelector.Cities.Add(cityListItem);
            CitySelector.SelectedCity = cityListItem;
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task Update_Execute()
    {
        if (string.IsNullOrEmpty(Name) || 
            string.IsNullOrEmpty(Surname) || 
            string.IsNullOrEmpty(Patronymic) ||
            string.IsNullOrEmpty(Phone) || 
            string.IsNullOrEmpty(BirthDate) || 
            CitySelector.SelectedCity is null)
        {
            ErrorMessage = "Заповніть всі необхідні поля";
            return;
        }

        var diseaseIds = Diseases
            .Where(d => d.IsSelected)
            .Select(d => d.Id)
            .ToList();

        var allergyIds = AllergyMultiSelector.GetSelectedAllergiesIds().ToList();

        if (!DateTime.TryParseExact(BirthDate, "d.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var birthDate))
        {
            ErrorMessage = "Некоректний формат дати народження";
            return;
        }

        var dto = new PatientUpdateDto
        {
            Id = _patientId,
            CityId = CitySelector.SelectedCity!.Id,
            Gender = Gender,
            Name = Name,
            Surname = Surname,
            Patronymic = Patronymic,
            BirthDate = birthDate,
            AllergyIds = allergyIds,
            DiseaseIds = diseaseIds,
            Phones = new List<PhoneCreateDto>()
            {
                new()
                {
                    PhoneNumber = Phone,
                    IsMain = true,
                    Tag = null
                }
            }
        };

        try
        {
            await _patientService.UpdateAsync(dto);
            _navigationService.NavigateTo(PageType.Patients);
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task<IEnumerable<CityListItemViewModel>> GetCitiesAsync()
    {
        return (await _commonService.GetCitiesAsync())
            .Select(c => new CityListItemViewModel
            {
                Id = c.Id,
                Name = c.Name
            });
    }

    private async Task<IEnumerable<DiseaseListItemViewModel>> GetDiseasesAsync()
    {
        var patientDiseases = await _patientService.GetDiseasesAsync(_patientId);
        
        return (await _commonService.GetDiseasesAsync())
            .Select(d => new DiseaseListItemViewModel
            {
                Id = d.Id,
                IsSelected = patientDiseases.Any(pd => d.Id == pd.Id),
                Name = d.Name
            });
    }

    private async Task<IEnumerable<AllergyListItemViewModel>> GetPatientAllergiesAsync(int patientId)
    {
        var patientAllergies = await _patientService.GetAllergiesAsync(patientId);

        return (await _commonService.GetAllergiesAsync())
            .Select(a => new AllergyListItemViewModel
            {
                Id = a.Id,
                IsSelected = patientAllergies.Any(pa => a.Id == pa.Id),
                Name = a.Name
            });
    }
}