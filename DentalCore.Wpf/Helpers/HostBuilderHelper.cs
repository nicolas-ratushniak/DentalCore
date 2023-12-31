using System;
using DentalCore.Domain.Abstract;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Configuration;
using DentalCore.Wpf.ViewModels;
using DentalCore.Wpf.ViewModels.Modals;
using DentalCore.Wpf.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DentalCore.Wpf.Helpers;

public static class HostBuilderHelper
{
    public static IHostBuilder AddViewModels(this IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<Func<PatientsViewModel>>(s => () => new PatientsViewModel(
                s.GetRequiredService<INavigationService>(),
                s.GetRequiredService<IModalService>(),
                s.GetRequiredService<IPatientService>()));

            services.AddSingleton<Func<int, PatientInfoViewModel>>(s => id => new PatientInfoViewModel(
                id,
                s.GetRequiredService<INavigationService>(),
                s.GetRequiredService<IPatientService>(),
                s.GetRequiredService<IVisitService>(),
                s.GetRequiredService<IPaymentService>()));

            services.AddSingleton<Func<PatientCreateViewModel>>(s => () => new PatientCreateViewModel(
                s.GetRequiredService<INavigationService>(),
                s.GetRequiredService<IPatientService>(),
                s.GetRequiredService<ICommonService>()));

            services.AddSingleton<Func<int, PatientUpdateViewModel>>(s => id => new PatientUpdateViewModel(
                id,
                s.GetRequiredService<INavigationService>(),
                s.GetRequiredService<IPatientService>(),
                s.GetRequiredService<ICommonService>()));

            services.AddSingleton<Func<VisitsViewModel>>(s => () => new VisitsViewModel(
                s.GetRequiredService<INavigationService>(),
                s.GetRequiredService<IVisitService>()));

            services.AddSingleton<Func<int, VisitInfoViewModel>>(s => id => new VisitInfoViewModel(
                id,
                s.GetRequiredService<IVisitService>()));

            services.AddSingleton<Func<int, VisitCreateViewModel>>(s => id => new VisitCreateViewModel(
                id,
                s.GetRequiredService<INavigationService>(),
                s.GetRequiredService<IVisitService>(),
                s.GetRequiredService<IPatientService>(),
                s.GetRequiredService<IUserService>(),
                s.GetRequiredService<IProcedureService>(),
                s.GetRequiredService<IPaymentService>()));

            services.AddSingleton<Func<ProceduresViewModel>>(s => () => new ProceduresViewModel(
                s.GetRequiredService<IProcedureService>(),
                s.GetRequiredService<IModalService>()));

            services.AddSingleton<Func<VisitsExportViewModel>>(s => () => new VisitsExportViewModel(
                s.GetRequiredService<IOptions<ExportOptions>>(),
                s.GetRequiredService<IExportService>(),
                s.GetRequiredService<IModalService>()));

            services.AddSingleton<Func<ProcedureCreateViewModel>>(s => () => new ProcedureCreateViewModel(
                s.GetRequiredService<IProcedureService>(),
                s.GetRequiredService<IModalService>()));
            
            services.AddSingleton<Func<int, ProcedureUpdateViewModel>>(s => id => new ProcedureUpdateViewModel(
                id,
                s.GetRequiredService<IProcedureService>(),
                s.GetRequiredService<IModalService>()));
            
            services.AddSingleton<Func<CityCreateViewModel>>(s => () => new CityCreateViewModel(
                s.GetRequiredService<ICommonService>(),
                s.GetRequiredService<IModalService>()));
            
            services.AddTransient<MainViewModel>();
        });

        return builder;
    }
}