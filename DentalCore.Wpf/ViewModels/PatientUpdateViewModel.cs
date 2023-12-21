using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Data.Models;
using DentalCore.Domain.Abstract;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Components;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class PatientUpdateViewModel : BaseViewModel
{
    private readonly ObservableCollection<CityListItemViewModel> _cities;
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
    private CityListItemViewModel? _selectedCity;
    private string _citySearchFilter = string.Empty;
    private bool _isCityListVisible;


    public ICommand CancelCommand { get; }
    public ICommand SubmitCommand { get; }

    public ICollectionView CityCollectionView { get; }

    public ObservableCollection<DiseaseListItemViewModel> Diseases { get; }
    public AllergySelectorComponent AllergySelector { get; }

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

    public string CitySearchFilter
    {
        get => _citySearchFilter;
        set
        {
            if (value == _citySearchFilter) return;
            _citySearchFilter = value;

            OnPropertyChanged();
            OnCityFilterChanged();
        }
    }

    public bool IsCityListVisible
    {
        get => _isCityListVisible;
        set
        {
            if (value == _isCityListVisible) return;
            _isCityListVisible = value;
            OnPropertyChanged();
        }
    }

    public CityListItemViewModel? SelectedCity
    {
        get => _selectedCity;
        set
        {
            if (Equals(value, _selectedCity)) return;
            _selectedCity = value;

            if (_selectedCity is not null)
            {
                CitySearchFilter = _selectedCity.Name;
                IsCityListVisible = false;
            }

            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    #region UiProperties

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

    #endregion

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
        _cities = new ObservableCollection<CityListItemViewModel>();
        Diseases = new ObservableCollection<DiseaseListItemViewModel>();

        AllergySelector = new AllergySelectorComponent();

        CityCollectionView = CollectionViewSource.GetDefaultView(_cities);
        CityCollectionView.Filter = o => o is CityListItemViewModel c &&
                                         c.Name.ToLower().StartsWith(CitySearchFilter.ToLower());

        CancelCommand = new RelayCommand<object>(_ =>
            _navigationService.NavigateTo(ViewType.Patients, null));

        SubmitCommand = new AsyncRelayCommand(Update_Execute);
        LoadedCommand = new AsyncRelayCommand(LoadData, ex => 
            MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private async Task LoadData()
    {
        foreach (var city in await GetCitiesAsync())
        {
            _cities.Add(city);
        }
        
        foreach (var allergy in await GetPatientAllergiesAsync(_patientId))
        {
            AllergySelector.Allergies.Add(allergy);
        }

        foreach (var disease in await GetDiseasesAsync())
        {
            Diseases.Add(disease);
        }
        
        var patient = await _patientService.GetAsync(_patientId);

        Name = patient.Name;
        Surname = patient.Surname;
        Patronymic = patient.Patronymic;
        BirthDate = patient.BirthDate.ToString("dd.MM.yyyy");
        Gender = patient.Gender;
        Phone = (await _patientService.GetPhonesAsync(patient.Id)).First().PhoneNumber;
        SelectedCity = _cities.Single(c => c.Id == patient.City.Id);
    }

    private async Task Update_Execute()
    {
        if (string.IsNullOrEmpty(Name) || 
            string.IsNullOrEmpty(Surname) || 
            string.IsNullOrEmpty(Patronymic) ||
            string.IsNullOrEmpty(Phone) || 
            string.IsNullOrEmpty(BirthDate) || 
            SelectedCity is null)
        {
            ErrorMessage = "Заповніть всі необхідні поля";
            return;
        }

        var diseaseIds = Diseases
            .Where(d => d.IsSelected)
            .Select(d => d.Id)
            .ToList();

        var allergyIds = AllergySelector.GetSelectedAllergiesIds().ToList();

        if (!DateTime.TryParseExact(BirthDate, "d.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var birthDate))
        {
            ErrorMessage = "Некоректний формат дати народження";
            return;
        }

        var dto = new PatientUpdateDto
        {
            Id = _patientId,
            CityId = SelectedCity!.Id,
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
            _navigationService.NavigateTo(ViewType.Patients, null);
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void OnCityFilterChanged()
    {
        var filter = CitySearchFilter;

        if (_selectedCity is null)
        {
            IsCityListVisible = !string.IsNullOrEmpty(filter);
        }
        else
        {
            if (filter == _selectedCity.Name)
            {
                IsCityListVisible = false;
                return;
            }

            IsCityListVisible = true;
            _selectedCity = null;
        }

        CityCollectionView.Refresh();
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