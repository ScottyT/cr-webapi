using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly CodeRedServices _employeesService;
    public EmployeesController(CodeRedServices empService) =>
        _employeesService = empService;

    [HttpGet]
    public async Task<List<Employee>> Get() =>
        await _employeesService.GetEmployees();
    
}

[ApiController]
[Route("api/[controller]")]

public class ReportsController : ControllerBase
{
    private readonly CodeRedServices _reportsService;
    public ReportsController(CodeRedServices reportService)
    {
        _reportsService = reportService;
    }
    
    [HttpGet]
    public async Task<List<Report>> Get() =>
        await _reportsService.GetReports();
        

    [HttpGet("{reportType}/{id}")]
    public async Task<ActionResult<Object>> GetReport(string id, string reportType)
    {
        var report = await _reportsService.GetReport(id, reportType);
        if (report is null)
        {
            return NotFound();
        }
        return report;
    }
    
    [HttpGet("{email}/employee")]
    public async Task<List<Report>> GetUserReports(string email)
    {
        var reports = await _reportsService.UserReports(email);
        return reports;
    }
    [HttpPost("{reportType}/{jobid}/new")]
    public async Task<IActionResult> Post([FromBody] Object report, string reportType, string jobid)
    {
        var createdReport = JsonSerializer.Serialize(report);
        await _reportsService.Create(reportType, createdReport);
        
       // return Created(domainName + "/" + reportType + "/" + jobid, report);
       //return Created(domainName + "/" + reportType + "/" + jobid, "Successfully created new report!");
       return CreatedAtAction(nameof(Get), "Successfully created report!");
    }
    [HttpPut("{reportType}/{jobId}/update")]
    public async Task<IActionResult> UpdateDispatch(Object updatedReport, string jobId, string reportType)
    {
        object? report = await _reportsService.GetReport(jobId, reportType);
        if (report is null) 
        {
            return NotFound();
        }
        
        var emailProperty = updatedReport.GetType().GetProperty("emailAddress");
        var modelType = report.GetType();
        switch (reportType) 
        {
            case "containment-sheet":
            case "tech-sheet":
                
                break;
            default:
                await _reportsService.Update(reportType, jobId);
                break;
        }
        
        return NoContent();
    }
}