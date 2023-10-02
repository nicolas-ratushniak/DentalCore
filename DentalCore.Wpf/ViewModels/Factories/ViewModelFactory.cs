﻿using System;
using DentalCore.Wpf.Services.Navigation;

namespace DentalCore.Wpf.ViewModels.Factories;

public class ViewModelFactory : IViewModelFactory
{
    private readonly Func<PatientsViewModel> _createPatientsVm;
    private readonly Func<int, PatientInfoViewModel> _createPatientInfoVm;
    private readonly Func<PatientCreateViewModel> _createPatientCreateVm;
    private readonly Func<int, PatientUpdateViewModel> _createPatientUpdateVm;
    private readonly Func<VisitsViewModel> _createVisitsVm;
    private readonly Func<int, VisitInfoViewModel> _createVisitInfoView;
    private readonly Func<int, VisitCreateViewModel> _createVisitCreateVm;

    public ViewModelFactory(
        Func<PatientsViewModel> createPatientsVm,
        Func<int, PatientInfoViewModel> createPatientInfoVm,
        Func<PatientCreateViewModel> createPatientCreateVm,
        Func<int, PatientUpdateViewModel> createPatientUpdateVm,
        Func<VisitsViewModel> createVisitsVm,
        Func<int, VisitInfoViewModel> createVisitInfoView,
        Func<int, VisitCreateViewModel> createVisitCreateVm)
    {
        _createPatientsVm = createPatientsVm;
        _createPatientInfoVm = createPatientInfoVm;
        _createPatientCreateVm = createPatientCreateVm;
        _createPatientUpdateVm = createPatientUpdateVm;
        _createVisitsVm = createVisitsVm;
        _createVisitInfoView = createVisitInfoView;
        _createVisitCreateVm = createVisitCreateVm;
    }

    public BaseViewModel CreateViewModel(ViewType viewType, object? viewParameter = null)
    {
        return viewType switch
        {
            ViewType.Patients => _createPatientsVm(),
            ViewType.PatientInfo => _createPatientInfoVm((int)viewParameter!),
            ViewType.PatientCreate => _createPatientCreateVm(),
            ViewType.PatientUpdate => _createPatientUpdateVm((int)viewParameter!),
            ViewType.Visits => _createVisitsVm(),
            ViewType.VisitInfo => _createVisitInfoView((int)viewParameter!),
            ViewType.VisitCreate => _createVisitCreateVm((int)viewParameter!),
            _ => throw new InvalidOperationException("Cannot create view model with this type")
        };
    }
}