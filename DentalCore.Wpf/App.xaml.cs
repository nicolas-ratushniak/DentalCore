using System;
using System.Linq;
using System.Windows;
using DentalCore.Data;
using DentalCore.Data.Models;
using DentalCore.Domain.DataExportServices;
using DentalCore.Domain.Services;
using DentalCore.Wpf.Abstract;
using DentalCore.Wpf.Configuration;
using DentalCore.Wpf.Helpers;
using DentalCore.Wpf.Services;
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
        const string configFile = "appsettings.json";
        GithubUpdateManager.RestoreSettingsFromBackup(configFile);
        
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddJsonFile(configFile))
            .ConfigureServices((builder, services) =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString), 
                    ServiceLifetime.Transient);
                
                services.Configure<UpdateOptions>(builder.Configuration.GetSection(UpdateOptions.Update));
                services.Configure<ExportOptions>(builder.Configuration.GetSection(ExportOptions.Export));

                services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
                services.AddSingleton<ICommonService, CommonService>();
                services.AddSingleton<IPatientService, PatientService>();
                services.AddSingleton<IProcedureService, ProcedureService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IVisitService, VisitService>();
                services.AddSingleton<IPaymentService, PaymentService>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IAuthenticationService, AuthenticationService>();
                services.AddSingleton<IExportService, ExcelExportService>();
                services.AddSingleton<IViewModelFactory, ViewModelFactory>();

                services.AddScoped<MainWindow>(s =>
                    new MainWindow(s.GetRequiredService<MainViewModel>()));
            })
            .AddViewModels()
            .Build();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();

        var args = Environment.GetCommandLineArgs();

        if (args is [_, "-updateDb"])
        {
            await using var context = AppHost.Services.GetRequiredService<AppDbContext>();
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }
        
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