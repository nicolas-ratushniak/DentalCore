using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DentalCore.Domain.DataExportServices;
using DentalCore.Domain.Exceptions;
using DentalCore.Wpf.Commands;
using DentalCore.Wpf.Configuration;
using DentalCore.Wpf.Services.Navigation;
using Microsoft.Extensions.Options;

namespace DentalCore.Wpf.ViewModels;

public class VisitsExportViewModel : BaseViewModel
{
    private readonly IExportService _exportService;
    private readonly INavigationService _navigationService;
    private readonly ExportOptions _exportConfiguration;
    private string _fromDateInput;
    private string _toDateInput;
    private string? _errorMessage;
    public ICommand ExportVisitsCommand { get; }
    public ICommand CancelCommand { get; }

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

    public string FromDateInput
    {
        get => _fromDateInput;
        set
        {
            if (value == _fromDateInput) return;
            _fromDateInput = value;
            OnPropertyChanged();
        }
    }

    public string ToDateInput
    {
        get => _toDateInput;
        set
        {
            if (value == _toDateInput) return;
            _toDateInput = value;
            OnPropertyChanged();
        }
    }

    public VisitsExportViewModel(
        IOptions<ExportOptions> exportOptions,
        IExportService exportService,
        INavigationService navigationService)
    {
        _exportConfiguration = exportOptions.Value;
        _exportService = exportService;
        _navigationService = navigationService;

        var today = DateTime.Today;

        FromDateInput = today.ToString("d.MM.yyyy");
        ToDateInput = today.ToString("d.MM.yyyy");

        ExportVisitsCommand = new AsyncRelayCommand(ExportVisits);
        CancelCommand = new RelayCommand<object>(_ =>
            _navigationService.NavigateTo(ViewType.Patients, null));
    }

    private async Task ExportVisits()
    {
        if (!DateOnly.TryParseExact(FromDateInput, "d.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var fromDate))
        {
            ErrorMessage = "Некоректний формат початкової дати";
            return;
        }

        if (!DateOnly.TryParseExact(ToDateInput, "d.MM.yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out var toDate))
        {
            ErrorMessage = "Некоректний формат кінцевої дати";
            return;
        }

        try
        {
            var dirPath = _exportConfiguration.DefaultDirPath;

            if (string.IsNullOrEmpty(dirPath))
            {
                MessageBox.Show("Не вказано шлях для зберігання звітів. Звернітся до розробника");
                return;
            }

            await _exportService.ExportVisitsAsync(fromDate, toDate, dirPath);
            _navigationService.NavigateTo(ViewType.Patients, null);

            MessageBox.Show($"Успішно створено звіт у папці {dirPath}", "Успіх!", MessageBoxButton.OK);
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (NoDataToExportException)
        {
            ErrorMessage = "Жодних візитів за цей період";
        }
    }
}