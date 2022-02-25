using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RestSharp;
using System.Net.Mime;
using System.Text.Json;
using System.Threading;
using System.Web;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("read:reports")]
public class ReportsController : ControllerBase
{
    private readonly CodeRedServices _reportsService;
    private AuthServices _authService;
    private readonly IMongoDbClient _mongoService;
    
    public ReportsController(CodeRedServices reportService, AuthServices authService, IMongoDbClient mongoService)
    {
        _authService = authService;
        _reportsService = reportService;
        _mongoService = mongoService;
    }
    
    [HttpGet]
    public async Task<List<Report>> Get()
    {
        return await _mongoService.GetReports();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Object>> GetReport(string id)
    {
        var report = await _mongoService.GetReport(id);
        if (report is null)
        {
            return NotFound();
        }
        
        return report;
    }
    
    [HttpGet("{reportType}/{id}/certificate")]
    public ActionResult<CertificateOfCompletion> GetCertReport(string id, string reportType)
    {
        var report = _mongoService.GetContract(reportType, id).FirstOrDefault();
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
        var report = _mongoService.GetContract(reportType, id).FirstOrDefault();
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
        var reports = await _mongoService.UserReports(email);
        return reports;
    }

    [HttpPost("{reportType}/{jobid}/new")]
    public async Task<IActionResult> Post([FromBody] Object report, string reportType, string jobid)
    {
        var createdReport = JsonSerializer.Serialize(report);
        await _mongoService.CreateReport(reportType, createdReport);
        
        return CreatedAtAction(nameof(Get), "Successfully created report!");
    }

    [HttpPost("psychrometric-chart/update-chart")]
    public async Task<IActionResult> CreatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _mongoService.UpdatePsychrometricChart(r, report, "new");
        return CreatedAtAction(nameof(Get), "Psychrometric chart saved successfully!");
    }

    [HttpPost("psychrometric-chart/update-progress")]
    public async Task<IActionResult> UpdatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _mongoService.UpdatePsychrometricChart(r, report, "update");
        return CreatedAtAction(nameof(Get), "Psychrometric chart updated successfully!");
    }

    [HttpPut("{reportType}/{id}/update")]
    // Only used for the containment-sheet, tech-sheet, inventory-logs, and atmospheric-readings
    public async Task<IActionResult> ReportsThatGetUpdated(Object updatedReport, string id, string reportType)
    {
        object? report = await _mongoService.GetReport(id);
        if (report is null) 
        {
            return NotFound();
        }
        
        switch (reportType) 
        {
            case "containment-sheet":
            case "tech-sheet":
                await _mongoService.UpdateReport(reportType, id);
                break;
            default:
                await _mongoService.UpdateReport(reportType, id);
                break;
        }
        
        return NoContent();
    }
}