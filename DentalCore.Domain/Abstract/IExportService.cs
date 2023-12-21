namespace DentalCore.Domain.Abstract;

public interface IExportService
{
    public Task ExportVisitsAsync(DateOnly from, DateOnly to, string dirPath);
}