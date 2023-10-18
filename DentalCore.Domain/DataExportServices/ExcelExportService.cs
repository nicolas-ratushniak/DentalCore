﻿using System.ComponentModel.DataAnnotations;
using DentalCore.Data.Models;
using DentalCore.Domain.Exceptions;
using DentalCore.Domain.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DentalCore.Domain.DataExportServices;

public class ExcelExportService : IExportService
{
    private class VisitExportDto
    {
        public DateTime Date { get; set; }
        public string PatientInfo { get; set; }
        public string DoctorInfo { get; set; }
        public int TotalSum { get; set; }
        public int ActuallyPayed { get; set; }
    }

    private readonly IVisitService _visitService;
    private readonly IPatientService _patientService;
    private readonly IUserService _userService;
    private readonly IPaymentService _paymentService;

    public ExcelExportService(
        IVisitService visitService,
        IPatientService patientService,
        IUserService userService,
        IPaymentService paymentService)
    {
        _visitService = visitService;
        _patientService = patientService;
        _userService = userService;
        _paymentService = paymentService;
    }

    public async Task ExportVisitsAsync(DateOnly from, DateOnly to, string dirPath)
    {
        if (from > to)
        {
            throw new ValidationException("Кінцева дата не має бути передувати початовій");
        }

        if (to > DateOnly.FromDateTime(DateTime.Now))
        {
            throw new ValidationException("Кінцева дата не має випереджати сьогоднішній день");
        }
        
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var fileName = string.Format("Звіт {0}-{1}.xlsx",
            from.ToString("d.MM.yyyy"),
            to.ToString("d.MM.yyyy"));

        var filePath = Path.Combine(dirPath, fileName);

        var visits = (await GetVisitsForExportAsync(from, to)).ToList();

        if (!visits.Any())
        {
            throw new NoDataToExportException("Жодних візитів за цей період");
        }

        using (var package = new ExcelPackage(filePath))
        {
            var sheetsCount = package.Workbook.Worksheets.Count;
            var sheet = package.Workbook.Worksheets.Add($"Звіт {sheetsCount + 1}");

            // fill header
            sheet.Cells[1, 2].Value = "Дата";
            sheet.Cells[1, 3].Value = "Пацієнт";
            sheet.Cells[1, 4].Value = "Лікар";
            sheet.Cells[1, 5].Value = "До сплати";
            sheet.Cells[1, 6].Value = "Сплачено";

            sheet.Cells[1, 2, 1, 6].Style.Font.Bold = true;
            
            // fill contents
            var tableEndRow = visits.Count + 1;
            
            sheet.Cells[2, 2].LoadFromCollection(visits);
            sheet.Cells[2, 1, tableEndRow, 1].FillNumber(1);
            
            sheet.Cells[2, 2, tableEndRow, 2].Style.Numberformat.Format = "dd.mm.yyy";

            // fill summary
            sheet.Cells[tableEndRow + 2, 4].Value = "Всього";
            sheet.Cells[tableEndRow + 2, 5].Formula = $"SUM(E2:E{tableEndRow})";
            sheet.Cells[tableEndRow + 2, 6].Formula = $"SUM(F2:F{tableEndRow})";

            sheet.Cells[tableEndRow + 2, 4, tableEndRow + 2, 6].Style.Font.Bold = true;
            
            // tweak the table
            sheet.Calculate();
            sheet.Cells.AutoFitColumns(0);
            
            for (int i = 1; i <= sheet.Dimension.End.Column; i++)
            {
                sheet.Column(i).Width += 3;
            }
            
            // set borders
            var table = sheet.Cells[1, 1, tableEndRow, sheet.Dimension.End.Column];
            
            table.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            table.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            table.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            table.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            sheet.Cells[tableEndRow + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            sheet.Cells[tableEndRow + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Medium;
            sheet.Cells[tableEndRow + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            
            sheet.Cells[tableEndRow + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            sheet.Cells[tableEndRow + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            
            sheet.Cells[tableEndRow + 2, 6].Style.Border.Top.Style = ExcelBorderStyle.Medium;
            sheet.Cells[tableEndRow + 2, 6].Style.Border.Right.Style = ExcelBorderStyle.Medium;
            sheet.Cells[tableEndRow + 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            
            await package.SaveAsync();
        }
    }

    private async Task<IEnumerable<VisitExportDto>> GetVisitsForExportAsync(DateOnly from, DateOnly to)
    {
        var fromDateTime = from.ToDateTime(TimeOnly.MinValue);
        var toDateTime = to.ToDateTime(TimeOnly.MaxValue);
        
        var visits = (await _visitService.GetAllAsync())
            .Where(visit =>
                visit.CreatedOn >= fromDateTime &&
                visit.CreatedOn <= toDateTime)
            .OrderBy(v => v.CreatedOn)
            .ToList();

        List<(int, int)> payedPerVisit = new();

        foreach (var visit in visits)
        {
            payedPerVisit.Add((visit.Id, await _paymentService.GetMoneyPayedForVisitAsync(visit.Id)));
        }

        var patientInfos = (await _patientService.GetAllAsync())
            .Where(patient => visits.Any(v => v.PatientId == patient.Id))
            .Select(p => (p.Id, $"{p.Surname} {p.Name} {p.Patronymic}"));

        var doctorInfos = (await _userService.GetAllAsync())
            .Where(user =>
                user.Role == UserRole.Doctor &&
                visits.Any(v => v.DoctorId == user.Id))
            .Select(d => (d.Id, $"{d.Surname} {d.Name}"));

        return visits.Select(visit =>
            new VisitExportDto
            {
                Date = visit.CreatedOn,
                PatientInfo = patientInfos.Single(p => p.Id == visit.PatientId).Item2,
                DoctorInfo = doctorInfos.Single(d => d.Id == visit.DoctorId).Item2,
                TotalSum = visit.TotalPrice,
                ActuallyPayed = payedPerVisit.Single(p => p.Item1 == visit.Id).Item2
            });
    }
}