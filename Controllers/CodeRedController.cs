using cr_app_webapi.Models;
using cr_app_webapi.Services;
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
    
    [HttpGet("{teamid}")]
    public async Task<ActionResult<Employee>> Get(string teamid)
    {
        var employee = await _employeesService.GetEmployee(teamid);
        if (employee is null)
        {
            return NotFound();
        }
        return employee;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(Employee newEmployee)
    {
        var jsonstring = JsonSerializer.Serialize(newEmployee);
        await _employeesService.CreateEmployee(jsonstring);
        return CreatedAtAction(nameof(Get), new { _id = newEmployee._id }, newEmployee);
    }
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
    
    [HttpGet("{reportType}/{id}/certificate")]
    public ActionResult<Certificate> GetCertReport(string id, string reportType)
    {
        var report = _reportsService.GetContract(reportType, id).FirstOrDefault();
        if (report is null)
        {
            return NoContent();
        }
        var result = (Certificate)report;
        result.Cert.creditCard = result.creditCard;
        return result;
    }
    [HttpGet("{reportType}/{id}/aob")]
    public ActionResult<AssignmentOfBenefits> GetAobReport(string id, string reportType)
    {
        var report = _reportsService.GetContract(reportType, id).FirstOrDefault();
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
        var reports = await _reportsService.UserReports(email);
        return reports;
    }

    [HttpPost("{reportType}/{jobid}/new")]
    public async Task<IActionResult> Post([FromBody] Object report, string reportType, string jobid)
    {
        var createdReport = JsonSerializer.Serialize(report);
        await _reportsService.Create(reportType, createdReport);
        
        return CreatedAtAction(nameof(Get), "Successfully created report!");
    }

    [HttpPost("psychrometric-chart/update-chart")]
    public async Task<IActionResult> CreatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _reportsService.UpdatePsychrometricChart(r, report, "new");
        return CreatedAtAction(nameof(Get), "Psychrometric chart saved successfully!");
    }

    [HttpPost("psychrometric-chart/update-progress")]
    public async Task<IActionResult> UpdatePsychrometric(Psychrometric report)
    {
        var r = JsonSerializer.Serialize(report);
        await _reportsService.UpdatePsychrometricChart(r, report, "update");
        return CreatedAtAction(nameof(Get), "Psychrometric chart updated successfully!");
    }

    [HttpPut("{reportType}/{jobId}/update")]
    // Only used for the containment-sheet, tech-sheet, inventory-logs, and atmospheric-readings
    public async Task<IActionResult> ReportsThatGetUpdated(Object updatedReport, string jobId, string reportType)
    {
        object? report = await _reportsService.GetReport(jobId, reportType);
        if (report is null) 
        {
            return NotFound();
        }
        
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