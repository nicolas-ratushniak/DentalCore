namespace DentalCore.Domain.DataExportServices;

public interface IExportService
{
    public Task ExportVisitsAsync(DateOnly from, DateOnly to, string dirPath);
}