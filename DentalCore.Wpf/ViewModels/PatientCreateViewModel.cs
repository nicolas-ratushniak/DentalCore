﻿using System;
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
using DentalCore.Wpf.ViewModels.Inners;

namespace DentalCore.Wpf.ViewModels;

public class PatientCreateViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IPatientService _patientService;
    private readonly ICommonService _commonService;
    private string _name;
    private string _surname;
    private string _patronymic;
    private Gender _gender;
    private string _phone;
    private string _allergyNamesInput;
    private string _citySearchFilter = string.Empty;
    private bool _isCityListVisible;
    private CityListItemViewModel? _selectedCity;
    private string _birthDate;
    private string? _errorMessage;

    public ICommand CancelCommand { get; set; }
    public ICommand SubmitCommand { get; set; }

    public ICollectionView CityCollectionView { get; set; }

    public ObservableCollection<DiseaseListItemViewModel> Diseases { get; set; }

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

    public string AllergyNamesInput
    {
        get => _allergyNamesInput;
        set
        {
            if (value == _allergyNamesInput) return;
            _allergyNamesInput = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public PatientCreateViewModel(INavigationService navigationService, IPatientService patientService,
        ICommonService commonService)
    {
        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");
        
        _navigationService = navigationService;
        _patientService = patientService;
        _commonService = commonService;

        Diseases = new ObservableCollection<DiseaseListItemViewModel>(GetDiseases());

        CityCollectionView = CollectionViewSource.GetDefaultView(GetCities());
        CityCollectionView.Filter = o => o is CityListItemViewModel c &&
                                         c.Name.ToLower().StartsWith(CitySearchFilter.ToLower());

        CancelCommand = new RelayCommand<object>(_ =>
            _navigationService.NavigateTo(ViewType.Patients, null));

        SubmitCommand = new RelayCommand<object>(Add_Execute, Add_CanExecute);
    }

    private bool Add_CanExecute(object obj)
    {
        return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname) && !string.IsNullOrEmpty(Patronymic) &&
               !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(BirthDate) && SelectedCity != null;
    }

    private void Add_Execute(object obj)
    {
        var diseaseIds = Diseases
            .Where(d => d.IsSelected)
            .Select(d => d.Id);

        var allergyNames = string.IsNullOrEmpty(AllergyNamesInput)
            ? null
            : AllergyNamesInput.Split(Environment.NewLine,
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (!DateTime.TryParse(BirthDate, CultureInfo.CurrentCulture, out var birthDate))
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
            Phone = Phone,
            BirthDate = birthDate,
            AllergyNames = allergyNames,
            DiseaseIds = diseaseIds
        };

        try
        {
            _patientService.Add(dto);
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
        return _commonService.GetDiseases()
            .Select(d => new DiseaseListItemViewModel
            {
                Id = d.Id,
                IsSelected = false,
                Name = d.Name
            });
    }
}