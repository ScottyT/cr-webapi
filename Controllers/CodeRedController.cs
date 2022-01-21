using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Text.Json;
using System.Reflection;

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
    public ReportsController(CodeRedServices reportService) =>
        _reportsService = reportService;
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

    [HttpPost("dispatch/new")]
    public async Task<IActionResult> PostDispatch(Dispatch report)
    {
        report.createdAt = DateTime.Now;
        report.updatedAt = DateTime.Now;
        await _reportsService.Create("dispatch", report);
        return CreatedAtAction(nameof(Get), new { _id = report.Id }, report);
    }

    [HttpPut("dispatch/{jobId}/update")]
    public async Task<IActionResult> UpdateDispatch(Dispatch updatedReport, string jobId)
    {
        var report = await _reportsService.GetReport(jobId, "dispatch");
        //Dispatch? fetchedReport = JsonSerializer.Deserialize<Dispatch>(report.ToJson());
        if (report is null) 
        {
            return NotFound();
        }
       /*  IList<System.Reflection.PropertyInfo> props = new List<System.Reflection.PropertyInfo>(report.GetType().GetProperties());
        foreach (PropertyInfo prop in props)
        {
            object? propValue = prop.GetValue(report, null);
        } */
        //var propertyInfo = report.GetType().GetProperty("")
        updatedReport.updatedAt = DateTime.Now;
        /* updatedReport.Id = fetchedReport.Id;
        updatedReport.createdAt = fetchedReport.createdAt; */
        await _reportsService.Update("dispatch", jobId, updatedReport);
        return NoContent();
    }
    [HttpPost("rapid-response/new")]
    public async Task<IActionResult> PostRapidResponse([FromBody] Report report)
    {
        await _reportsService.Create("rapid-response", report);
        return CreatedAtAction(nameof(GetReport), new { _id = report.Id }, report);
    }
}