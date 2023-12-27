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
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.ViewModels.Components;
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels.Pages;

public class PatientCreateViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IPatientService _patientService;
    private readonly ICommonService _commonService;

    private string _name;
    private string _surname;
    private string _patronymic;
    private Gender _gender = Gender.Male;
    private string _phone;
    private string _citySearchFilter = string.Empty;
    private bool _isCityListVisible;
    private CityListItemViewModel? _selectedCity;
    private string _birthDate;
    private string? _errorMessage;
    private readonly ObservableCollection<CityListItemViewModel> _cities;

    public ICommand CancelCommand { get; set; }
    public ICommand SubmitCommand { get; set; }

    public ICollectionView CityCollectionView { get; }

    public ObservableCollection<DiseaseListItemViewModel> Diseases { get; }

    public AllergySelectorComponent AllergySelector { get; }

    #region SearchAndFilter

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

    #endregion

    #region UiProperties

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

    #endregion

    public PatientCreateViewModel(
        INavigationService navigationService,
        IPatientService patientService,
        ICommonService commonService)
    {
        _navigationService = navigationService;
        _patientService = patientService;
        _commonService = commonService;
        _cities = new ObservableCollection<CityListItemViewModel>();
        Diseases = new ObservableCollection<DiseaseListItemViewModel>();

        AllergySelector = new AllergySelectorComponent();

        CityCollectionView = CollectionViewSource.GetDefaultView(_cities);

        CityCollectionView.Filter = o =>
            o is CityListItemViewModel c &&
            c.Name.ToLower().StartsWith(CitySearchFilter.ToLower());

        CancelCommand = new RelayCommand<object>(_ =>
            _navigationService.NavigateTo(PageType.Patients, null));

        SubmitCommand = new AsyncRelayCommand(Add_Execute);

        LoadedCommand = new AsyncRelayCommand(LoadData, ex => 
            MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private async Task LoadData()
    {
        foreach (var disease in await GetDiseasesAsync())
        {
            Diseases.Add(disease);
        }

        foreach (var city in await GetCitiesAsync())
        {
            _cities.Add(city);
        }

        foreach (var allergy in await GetAllergiesAsync())
        {
            AllergySelector.Allergies.Add(allergy);
        }
    }

    private async Task Add_Execute()
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

        var dto = new PatientCreateDto
        {
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
            var id = await _patientService.AddAsync(dto);
            _navigationService.NavigateTo(PageType.PatientInfo, id);
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
        return (await _commonService.GetDiseasesAsync())
            .Select(d => new DiseaseListItemViewModel
            {
                Id = d.Id,
                IsSelected = false,
                Name = d.Name
            });
    }
    
    private async Task<IEnumerable<AllergyListItemViewModel>> GetAllergiesAsync()
    {
        return (await _commonService.GetAllergiesAsync())
            .Select(a => new AllergyListItemViewModel
            {
                Id = a.Id,
                IsSelected = false,
                Name = a.Name
            });
    }
}