using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.Json;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("read:reports")]
public class ReportsController : ControllerBase
{
    private readonly IMongoRepo<Report, Report> _report;
    private readonly ReportsService _reportsService;
    private readonly IMongoRepo<CreditCard, CreditCard> _creditCard;
    private readonly IMongoRepo<CertificateOfCompletion, CreditCard> _coc;
    private readonly IMongoRepo<AssignmentOfBenefits, CreditCard> _aob;
    private readonly IMongoRepo<InventoryModel, InventoryImage> _contentInventory;
    private readonly ICodeRedDatabaseSettings _settings;

    public ReportsController(IMongoRepo<Report, Report> mongoReport, ReportsService reportsService, IMongoRepo<CreditCard, CreditCard> creditCard,
        ICodeRedDatabaseSettings settings, IMongoRepo<CertificateOfCompletion, CreditCard> coc, IMongoRepo<AssignmentOfBenefits, CreditCard> aob, 
        IMongoRepo<InventoryModel, InventoryImage> contentInventory)
    {
        _settings = settings;
        _report = mongoReport;
        _reportsService = reportsService;
        _creditCard = creditCard;
        _coc = coc;
        _aob = aob;
        _contentInventory = contentInventory;
    }
    [HttpGet]
    public IQueryable<Report> GetAll() =>
        _report.AsQueryable();

    [HttpGet("details/{reportType}/{id}")]
    public async Task<ActionResult<Object>> GetReport(string id, string reportType)
    {
        var report = await _reportsService.GetReport(id, reportType);
        /* MongoRepo<CaseFile> cf = new MongoRepo<CaseFile>(_settings);
        var test = cf.FilterBy(
            filter => filter.JobId == id && filter.ReportType == reportType
        ).FirstOrDefault(); */
        if (reportType == "personal-content-inventory")
        {
            report = _contentInventory.FindAndJoin<InventoryModel>(
                f => f.JobId == id && f.ReportType == reportType,
                l => l.JobId, foreign => foreign.JobId, j => j.inventoryImages
            ).Cast<object>();
        }
        if (report is null)
        {
            return NotFound();
        }
        if (reportType == "personal-content-inventory")
        {

        }

        return report;
    }

    [HttpGet("{reportType}/{id}/aob")]
    public ActionResult<AssignmentOfBenefits> GetAobReport(string id, string reportType)
    {
        var report = _aob.FindAndJoin<AssignmentOfBenefits>(
            f => f.JobId == id && f.ReportType == reportType,
            l => l.cardNumber, foreign => foreign.cardNumber,
            j => j.creditCard
        ).FirstOrDefault();
        if (report is null)
        {
            return NotFound("No report found of this job id");
        }

        return report;
    }

    [HttpGet("{reportType}/{id}/certificate")]
    public ActionResult<CertificateOfCompletion> GetCertReport(string id, string reportType)
    {
        var report = _coc.FindAndJoin<CertificateOfCompletion>(
            f => f.JobId == id && f.ReportType == reportType,
            x => x.cardNumber, y => y.cardNumber, x => x.creditCard
        ).FirstOrDefault();
        if (report is null)
        {
            return NotFound("No report found of this job id");
        }

        return report;
    }

    [HttpGet("user/{email}")]
    public ActionResult<Report> GetUserReports(string email)
    {
        var report = _report.FilterBy(
            filter => filter.teamMember.email == email
        ).FirstOrDefault();
        if (report is null)
        {
            return NotFound();
        }
        return report;
    }

    [HttpPost("{reportType}/new")]
    public async Task<IActionResult> Post(Object newReport, string reportType, string jobid)
    {
        var report = _report.FilterBy(
            filter => filter.JobId == jobid && filter.formType != "case-report"
        ).FirstOrDefault();
        if (report is not null)
        {
            return Ok(new { error = true, message = "A report of this type and job id already exsits." });
        }

        var createdReport = JsonSerializer.Serialize(newReport);
        await _reportsService.CreateReport(reportType, createdReport);

        return CreatedAtAction(nameof(GetAll), "Successfully created report!");
    }

    [HttpPut("{reportType}/{id}/update")]
    // Only used for the inventory-logs, and atmospheric-readings
    public async Task<IActionResult> ReportsThatGetUpdated(Object updatedReport, string id, string reportType)
    {
        /* object? report = await GetReport(id, reportType);
        if (report is null)
        {
            return NotFound();
        } */

        var reportBody = JsonSerializer.Serialize(updatedReport);
        await _reportsService.UpdateReport(reportBody, reportType, id);

        return Ok("Successfully submitted the report!");
    }

    [HttpPost("psychrometric-chart/update-chart")]
    public async Task<IActionResult> CreatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _reportsService.UpdatePsychrometricChart(r, report, "new");
        return CreatedAtAction(nameof(GetAll), "Psychrometric chart saved successfully!");
    }

    [HttpPost("psychrometric-chart/update-progress")]
    public async Task<IActionResult> UpdatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _reportsService.UpdatePsychrometricChart(r, report, "update");
        return CreatedAtAction(nameof(GetAll), "Psychrometric chart updated successfully!");
    }
    /* 
    [HttpGet("{email}/employee")]
    public async Task<List<Report>> GetUserReports(string email)
    {
        var reports = await _reportService.UserReports(email);
        return reports;
    }

    [HttpPut("{reportType}/{id}/update")]
    // Only used for the containment-sheet, tech-sheet, inventory-logs, and atmospheric-readings
    public async Task<IActionResult> ReportsThatGetUpdated(Object updatedReport, string id, string reportType)
    {
        object? report = await _reportService.GetReport(id);
        if (report is null) 
        {
            return NotFound();
        }
        
        switch (reportType) 
        {
            case "containment-sheet":
            case "tech-sheet":
                await _reportService.UpdateReport(reportType, id);
                break;
            default:
                await _reportService.UpdateReport(reportType, id);
                break;
        }
        
        return NoContent();
    } */
}