using System;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.ViewModels.Modals;
using DentalCore.Wpf.ViewModels.Pages;

namespace DentalCore.Wpf.ViewModels;

public class ViewModelFactory : IViewModelFactory
{
    private readonly Func<PatientsViewModel> _createPatientsVm;
    private readonly Func<int, PatientInfoViewModel> _createPatientInfoVm;
    private readonly Func<PatientCreateViewModel> _createPatientCreateVm;
    private readonly Func<int, PatientUpdateViewModel> _createPatientUpdateVm;
    private readonly Func<VisitsViewModel> _createVisitsVm;
    private readonly Func<int, VisitInfoViewModel> _createVisitInfoView;
    private readonly Func<int, VisitCreateViewModel> _createVisitCreateVm;
    private readonly Func<VisitsExportViewModel> _createVisitsExportVm;
    private readonly Func<ProcedureCreateViewModel> _createProcedureCreateVm;
    private readonly Func<int, ProcedureUpdateViewModel> _createProcedureUpdateVm;
    private readonly Func<CityCreateViewModel> _createCityCreateVm;
    private readonly Func<ProceduresViewModel> _createProceduresVm;

    public ViewModelFactory(
        Func<PatientsViewModel> createPatientsVm,
        Func<int, PatientInfoViewModel> createPatientInfoVm,
        Func<PatientCreateViewModel> createPatientCreateVm,
        Func<int, PatientUpdateViewModel> createPatientUpdateVm,
        Func<VisitsViewModel> createVisitsVm,
        Func<int, VisitInfoViewModel> createVisitInfoView,
        Func<int, VisitCreateViewModel> createVisitCreateVm,
        Func<ProceduresViewModel> createProceduresVm,
        Func<VisitsExportViewModel> createVisitsExportVm,
        Func<ProcedureCreateViewModel> createProcedureCreateVm,
        Func<int, ProcedureUpdateViewModel> createProcedureUpdateVm,
        Func<CityCreateViewModel> createCityCreateVm)
    {
        _createPatientsVm = createPatientsVm;
        _createPatientInfoVm = createPatientInfoVm;
        _createPatientCreateVm = createPatientCreateVm;
        _createPatientUpdateVm = createPatientUpdateVm;
        _createVisitsVm = createVisitsVm;
        _createVisitInfoView = createVisitInfoView;
        _createVisitCreateVm = createVisitCreateVm;
        _createProceduresVm = createProceduresVm;
        
        _createVisitsExportVm = createVisitsExportVm;
        _createProcedureCreateVm = createProcedureCreateVm;
        _createProcedureUpdateVm = createProcedureUpdateVm;
        _createCityCreateVm = createCityCreateVm;
    }

    public BaseViewModel CreatePageViewModel(PageType pageType)
    {
        return pageType switch
        {
            PageType.Patients => _createPatientsVm(),
            PageType.PatientCreate => _createPatientCreateVm(),
            PageType.Visits => _createVisitsVm(),
            PageType.Procedures => _createProceduresVm(),
            _ => throw new InvalidOperationException("Cannot create view model with this type")
        };
    }
    
    public BaseViewModel CreatePageViewModel(PageType pageType, object viewParameter)
    {
        var id = (int)viewParameter;
        
        return pageType switch
        {
            PageType.PatientInfo => _createPatientInfoVm(id),
            PageType.PatientUpdate => _createPatientUpdateVm(id),
            PageType.VisitInfo => _createVisitInfoView(id),
            PageType.VisitCreate => _createVisitCreateVm(id),
            _ => throw new InvalidOperationException("Cannot create view model with this type")
        };
    }

    public BaseViewModel CreateModalViewModel(ModalType modalType)
    {
        return modalType switch
        {
            ModalType.VisitReport => _createVisitsExportVm(),
            ModalType.ProcedureCreate => _createProcedureCreateVm(),
            ModalType.CityCreate => _createCityCreateVm(),
            _ => throw new InvalidOperationException("Cannot create view model with this type")
        };
    }

    public BaseViewModel CreateModalViewModel(ModalType modalType, object modalParameter)
    {
        var id = (int)modalParameter;

        return modalType switch
        {
            ModalType.ProcedureUpdate => _createProcedureUpdateVm(id),
            _ => throw new InvalidOperationException("Cannot create view model with this type")
        };
    }
}