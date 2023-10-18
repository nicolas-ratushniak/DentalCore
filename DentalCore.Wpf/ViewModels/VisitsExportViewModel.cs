using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DentalCore.Domain.DataExportServices;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Configuration;
using Microsoft.Extensions.Options;

namespace DentalCore.Wpf.ViewModels;

public class VisitsExportViewModel : BaseViewModel
{
    private readonly IExportService _exportService;
    private readonly ExportOptions _exportConfiguration;
    public ICommand ExportVisitsCommand { get; }

    public VisitsExportViewModel(IOptions<ExportOptions> exportOptions, IExportService exportService)
    {
        _exportConfiguration = exportOptions.Value;
        _exportService = exportService;
        
        ExportVisitsCommand = new AsyncRelayCommand(ExportVisits);
    }
    
    private async Task ExportVisits()
    {
        await _exportService.ExportVisitsAsync(
            new DateOnly(2023, 9, 1),
            new DateOnly(2023, 10, 17),
            _exportConfiguration.DefaultDirPath);
    }
}