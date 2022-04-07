using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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
    private readonly IMongoRepo<CertificateOfCompletion, CreditCard> _coc;
    private readonly IMongoRepo<AssignmentOfBenefits, CreditCard> _aob;
    private readonly IMongoRepo<InventoryModel, InventoryImage> _contentInventory;
    private readonly IMongoRepo<Logging, Logging> _logging;
    private readonly IMongoRepo<MoistureModel, MoistureModel> _moistureMap;

    public ReportsController(IMongoRepo<Report, Report> mongoReport, ReportsService reportsService, IMongoRepo<CertificateOfCompletion, CreditCard> coc,
        IMongoRepo<AssignmentOfBenefits, CreditCard> aob, IMongoRepo<InventoryModel, InventoryImage> contentInventory, IMongoRepo<Logging, Logging> logging,
        IMongoRepo<MoistureModel, MoistureModel> moistureMap)
    {
        _report = mongoReport;
        _reportsService = reportsService;
        _coc = coc;
        _aob = aob;
        _contentInventory = contentInventory;
        _logging = logging;
        _moistureMap = moistureMap;
    }
    [HttpGet]
    public IQueryable<Report> GetAll() =>
        _report.AsQueryable();

    [HttpGet("details/{reportType}/{id}")]
    public async Task<ActionResult<Object>> GetReport(string id, string reportType)
    {
        var report = await _reportsService.GetReport(id, reportType);
        if (reportType == "personal-content-inventory")
        {
            report = _contentInventory.FindAndJoin<InventoryModel>(
                f => f.JobId == id && f.ReportType == reportType,
                l => l.JobId, foreign => foreign.JobId, j => j.inventoryImages
            ).Cast<object>().FirstOrDefault();
        }
        if (reportType == "quantity-inventory-logs" || reportType == "atmospheric-readings")
        {
            report = _logging.FilterBy(
                f => f.JobId == id && f.ReportType == reportType
            ).Cast<object>().FirstOrDefault();
        }
        if (reportType == "moisture-map")
        {
            report = _moistureMap.FilterBy(
                f => f.JobId == id && f.ReportType == reportType
            ).Cast<object>().FirstOrDefault();
        }
        if (report is null)
        {
            return NotFound();
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

    [HttpPost("{reportType}/new")]
    public async Task<IActionResult> Post(Object newReport, string reportType, string jobid)
    {
        var report = _report.FilterBy(
            filter => filter.JobId == jobid && filter.formType != "case-report" && filter.ReportType == reportType
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
    // Only used for the inventory-logs, and atmospheric-readings, and personal-content-inventory and moisture-map
    public async Task<IActionResult> ReportsThatGetUpdated(Object updatedReport, string id, string reportType)
    {
        var reportBody = JsonSerializer.Serialize(updatedReport);
        await _contentInventory.BsonFindOneAndUpdate(id, reportType, reportBody, true);

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
    
    [HttpGet("{email}/employee")]
    public ActionResult<List<Report>> GetUserReports(string email)
    {
        var reports = _report.FilterBy(f => f.teamMember.email == email).ToList();
        return reports;
    }
}