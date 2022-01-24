using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using MongoDB.Bson;
using System;
using System.Text.Json;
using System.Reflection;
using System.Net;
using System.Net.Http.Json;

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
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ReportsController(CodeRedServices reportService, IHttpContextAccessor httpContextAccessor)
    {
        _reportsService = reportService;
        _httpContextAccessor = httpContextAccessor;
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

    [HttpPost("{reportType}/{jobid}/new")]
    public async Task<IActionResult> Post([FromBody] Object report, string reportType, string jobid)
    {
        switch (reportType)
        {
            case "dispatch":
                var createdReport = JsonSerializer.Serialize(report);
                var deserialized = JsonSerializer.Deserialize<Dispatch>(createdReport);
                await _reportsService.Create(reportType, createdReport);
                break;
        }
        
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