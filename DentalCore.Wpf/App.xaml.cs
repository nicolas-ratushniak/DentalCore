using System.Windows;
using DentalCore.Data;
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

            })
            .Build();
    }
}