using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("read:reports")]
public class ReportsController : ControllerBase
{
    //private readonly ReportService _reportService;
    private readonly IMongoRepo<Report> _report;
    private readonly ReportsService _reportsService;
    private readonly IMongoRepo<CreditCard> _creditCard;
    
    public ReportsController(IMongoRepo<Report> mongoReport, ReportsService reportsService, IMongoRepo<CreditCard> creditCard)
    {
        _report = mongoReport;
        _reportsService = reportsService;
        _creditCard = creditCard;
    }
    [HttpGet]
    public IQueryable<Report> GetAll() =>
        _report.AsQueryable();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Object>> GetReport(string id)
    {
        var report = await _reportsService.GetReport(id);
        
        if (report is null)
        {
            return NotFound();
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

    [HttpPost("{reportType}/{jobid}/new")]
    public async Task<IActionResult> Post([FromBody] Object report, string reportType, string jobid)
    {
        var createdReport = JsonSerializer.Serialize(report);
        await _reportsService.CreateReport(reportType, createdReport);
        
        return CreatedAtAction(nameof(GetAll), "Successfully created report!");
    }
    
    /* [HttpGet("{reportType}/{id}/certificate")]
    public ActionResult<CertificateOfCompletion> GetCertReport(string id, string reportType)
    {
        var report = _reportService.GetContract(reportType, id).FirstOrDefault();
        if (report is null)
        {
            return NoContent();
        }
        var result = (Certificate)report;
        result.Cert.creditCard = result.creditCard;
        return result.Cert;
    }
    [HttpGet("{reportType}/{id}/aob")]
    public ActionResult<AssignmentOfBenefits> GetAobReport(string id, string reportType)
    {
        var report = _reportService.GetContract(reportType, id).FirstOrDefault();
        if (report is null)
        {
            return NoContent();
        }
        var result = (Aob)report;
        result.AOB.creditCard = result.creditCard;
        return result.AOB;
    }
    [HttpGet("{email}/employee")]
    public async Task<List<Report>> GetUserReports(string email)
    {
        var reports = await _reportService.UserReports(email);
        return reports;
    }

    [HttpPost("psychrometric-chart/update-chart")]
    public async Task<IActionResult> CreatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _reportService.UpdatePsychrometricChart(r, report, "new");
        return CreatedAtAction(nameof(GetAll), "Psychrometric chart saved successfully!");
    }

    [HttpPost("psychrometric-chart/update-progress")]
    public async Task<IActionResult> UpdatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _reportService.UpdatePsychrometricChart(r, report, "update");
        return CreatedAtAction(nameof(GetAll), "Psychrometric chart updated successfully!");
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