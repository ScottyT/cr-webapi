using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc;

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
        await _employeesService.GetEmployeesAsync();
    
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
        await _reportsService.GetReportsAsync();
}