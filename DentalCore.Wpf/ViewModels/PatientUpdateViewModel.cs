using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DentalCore.Data.Models;
using DentalCore.Domain.Dto;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels.Components;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class PatientUpdateViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IPatientService _patientService;
    private readonly ICommonService _commonService;
    private readonly int _patientId;

    private string _name;
    private string _surname;
    private string _patronymic;
    private Gender _gender;
    private string _phone;
    private string _citySearchFilter = string.Empty;
    private bool _isCityListVisible;
    private CityListItemViewModel? _selectedCity;
    private string _birthDate;
    private string? _errorMessage;

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

        var patient = patientService.Get(id);
        var cities = GetCities().ToList();

        Name = patient.Name;
        Surname = patient.Surname;
        Patronymic = patient.Patronymic;
        BirthDate = patient.BirthDate.ToString("dd.MM.yyyy");
        Gender = patient.Gender;
        Phone = _patientService.GetPhones(patient.Id).First().PhoneNumber;
        SelectedCity = cities.Single(c => c.Id == patient.CityId);

        Diseases = new ObservableCollection<DiseaseListItemViewModel>(GetDiseases());
        AllergySelector = new AllergySelectorComponent(_patientId, patientService, commonService);

        CityCollectionView = CollectionViewSource.GetDefaultView(cities);
        CityCollectionView.Filter = o => o is CityListItemViewModel c &&
                                         c.Name.ToLower().StartsWith(CitySearchFilter.ToLower());

        CancelCommand = new RelayCommand<object>(_ =>
            _navigationService.NavigateTo(ViewType.Patients, null));

        SubmitCommand = new RelayCommand<object>(Update_Execute, Update_CanExecute);
    }

    private bool Update_CanExecute(object obj)
    {
        return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname) && !string.IsNullOrEmpty(Patronymic) &&
               !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(BirthDate) && SelectedCity != null;
    }

    private void Update_Execute(object obj)
    {
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
            _patientService.Update(dto);
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
    
    private IEnumerable<CityListItemViewModel> GetCities()
    {
        return _commonService.GetCities()
            .Select(c => new CityListItemViewModel
            {
                Id = c.Id,
                Name = c.Name
            });
    }

    private IEnumerable<DiseaseListItemViewModel> GetDiseases()
    {
        var patientDiseases = _patientService.GetDiseases(_patientId);

        return _commonService.GetDiseases()
            .Select(d => new DiseaseListItemViewModel
            {
                Id = d.Id,
                IsSelected = patientDiseases.Any(dis => dis.Id == d.Id),
                Name = d.Name
            });
    }
}