using System.Windows;
using DentalCore.Data;
using DentalCore.Domain.Services;
using DentalCore.Wpf.ViewModels;
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

                services.AddSingleton<ICommonService, CommonService>();
                services.AddSingleton<IPatientService, PatientService>();
                services.AddSingleton<IProcedureService, ProcedureService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IVisitService, VisitService>();
                
                services.AddTransient<MainViewModel>();

                services.AddScoped<MainWindow>(s =>
                    new MainWindow(s.GetRequiredService<MainViewModel>()));

            })
            .Build();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
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