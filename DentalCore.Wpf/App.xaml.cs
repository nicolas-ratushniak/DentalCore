using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Services.Authentication;
using DentalCore.Wpf.Services.Navigation;
using DentalCore.Wpf.ViewModels;
using DentalCore.Wpf.ViewModels.Factories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DentalCore.Wpf;

public partial class App : Application
{
    private static readonly IHost AppHost;

    static App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.json"))
            .ConfigureServices((builder, services) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

                services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                services.AddSingleton<ICommonService, CommonService>();
                services.AddSingleton<IPatientService, PatientService>();
                services.AddSingleton<IProcedureService, ProcedureService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IVisitService, VisitService>();

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IAuthenticationService, AuthenticationService>();

                services.AddSingleton<IViewModelFactory, ViewModelFactory>();

                services.AddSingleton<Func<HomeViewModel>>(s => () => new HomeViewModel());
                services.AddSingleton<Func<LoginViewModel>>(s => () => new LoginViewModel());
                
                services.AddSingleton<Func<PatientsViewModel>>(s => () => new PatientsViewModel(
                    s.GetRequiredService<INavigationService>(),
                    s.GetRequiredService<IPatientService>()));
                
                services.AddSingleton<Func<int, PatientInfoViewModel>>(s => id => new PatientInfoViewModel(id));

                services.AddSingleton<Func<PatientCreateViewModel>>(s => () => new PatientCreateViewModel(
                    s.GetRequiredService<INavigationService>(),
                    s.GetRequiredService<IPatientService>(),
                    s.GetRequiredService<ICommonService>()));
                    
                services.AddSingleton<Func<int, PatientUpdateViewModel>>(s => id => new PatientUpdateViewModel(
                    id,
                    s.GetRequiredService<INavigationService>(),
                    s.GetRequiredService<IPatientService>(),
                    s.GetRequiredService<ICommonService>()));
                
                services.AddSingleton<Func<VisitsViewModel>>(s => () => new VisitsViewModel());
                services.AddSingleton<Func<int, VisitInfoViewModel>>(s => id => new VisitInfoViewModel(id));
                services.AddSingleton<Func<VisitCreateViewModel>>(s => () => new VisitCreateViewModel());
                
                services.AddTransient<MainViewModel>();

                services.AddScoped<MainWindow>(s =>
                    new MainWindow(s.GetRequiredService<MainViewModel>()));

            })
            .Build();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        Thread.CurrentThread.CurrentCulture = new CultureInfo("uk-UA");
        
        MainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();

        base.OnExit(e);
    }
}