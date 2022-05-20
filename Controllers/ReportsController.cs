using cr_app_webapi.Dto;
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
    private readonly IMongoRepo<Sketch, Sketch> _sketches;
    private readonly ReportsService _reportsService;
    private readonly IMongoRepo<CertificateOfCompletion, CreditCard> _coc;
    private readonly IMongoRepo<AssignmentOfBenefits, CreditCard> _aob;
    private readonly IMongoRepo<InventoryModel, InventoryImage> _contentInventory;
    private readonly IMongoRepo<Logging, Logging> _logging;
    private readonly IMongoRepo<MoistureModel, MoistureModel> _moistureMap;
    private readonly IMongoRepo<PsychrometricDto, Psychrometric> _psy;

    public ReportsController(IMongoRepo<Report, Report> mongoReport, ReportsService reportsService, IMongoRepo<CertificateOfCompletion, CreditCard> coc,
        IMongoRepo<AssignmentOfBenefits, CreditCard> aob, IMongoRepo<InventoryModel, InventoryImage> contentInventory, IMongoRepo<Logging, Logging> logging,
        IMongoRepo<MoistureModel, MoistureModel> moistureMap, IMongoRepo<Sketch, Sketch> sketches, IMongoRepo<PsychrometricDto, Psychrometric> psy)
    {
        _report = mongoReport;
        _reportsService = reportsService;
        _coc = coc;
        _aob = aob;
        _contentInventory = contentInventory;
        _logging = logging;
        _moistureMap = moistureMap;
        _sketches = sketches;
        _psy = psy;
    }
    [HttpGet]
    public IEnumerable<Report> GetAll()
    {
        var reports = _report.FilterBy(
            f => f.formType != "sketch-report"
        ).ToList();
        var sketches = _sketches.AsQueryable().ToList();
        ResultsDTO res = new ResultsDTO()
        {
            Reports = reports,
            Sketches = sketches
        };
        return reports;
    }

    [HttpGet("details/{reportType}/{id}")]
    public ActionResult<Object> GetReport(string id, string reportType)
    {
        var report = _report.FilterBy<Object>(
            f => f.JobId == id && f.ReportType == reportType,
            p => new { p }
        ).AsQueryable<Object>().FirstOrDefault();
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
        if (reportType.Contains("aob"))
        {
            report = _aob.FindAndJoin<AssignmentOfBenefits>(
                f => f.JobId == id && f.ReportType == reportType,
                l => l.cardNumber, foreign => foreign.cardNumber,
                j => j.creditCard
            ).Cast<object>().FirstOrDefault();
        }
        if (reportType.Contains("coc"))
        {
            report = _coc.FindAndJoin<CertificateOfCompletion>(
                f => f.JobId == id && f.ReportType == reportType,
                l => l.cardNumber, foreign => foreign.cardNumber,
                j => j.creditCard
            ).Cast<object>().FirstOrDefault();
        }
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
            filter => filter.JobId == jobid && filter.formType != "case-report" && filter.ReportType == reportType && filter.formType != "sketch-report"
        ).FirstOrDefault();
        if (report is not null)
        {
            return Conflict(new { error = true, message = "A report of this type and job id already exsits." });
        }

        var createdReport = JsonSerializer.Serialize(newReport);
        //await _reportsService.CreateReport(reportType, createdReport);
        await _report.BsonSaveOneAsync(createdReport);

        return CreatedAtAction(nameof(GetAll), "Successfully created report!");
    }

    [HttpPut("{reportType}/{id}/update")]
    // Only used for the inventory-logs, and atmospheric-readings, and personal-content-inventory and moisture-map
    public async Task<IActionResult> ReportsThatGetUpdated(Object updatedReport, string id, string reportType)
    {
        if (updatedReport is null)
        {
            return BadRequest(new { message = "Something bad happened" });
        }
        var reportBody = JsonSerializer.Serialize(updatedReport);
        await _contentInventory.BsonFindOneAndUpdate(id, reportType, reportBody, true);

        return Ok(new { message = "Successfully submitted the report!", result = updatedReport });
    }

    [HttpPost("psychrometric-chart/update-chart")]
    public async Task<IActionResult> CreatePsychrometric(Psychrometric report)
    {
        var JobProgress = report.jobProgress;
        var dto = new PsychrometricDto()
        {
            JobId = report.JobId,
            ReportType = report.ReportType,
            formType = report.formType,
            jobProgress = new List<JobProgress>()
            {
                JobProgress
            }
        };
        await _psy.GenericFindOneUpdate<PsychrometricDto>(
            f => f.JobId == report.JobId && f.ReportType == report.ReportType && f.formType == report.formType,
            dto, upsert: true, action: "new", setArrUpdate: Builders<PsychrometricDto>.Update.Push("jobProgress", JobProgress)
        );
        return CreatedAtAction(nameof(GetAll), "Psychrometric chart has been created!");
    }

    [HttpPost("psychrometric-chart/update-progress")]
    public async Task<IActionResult> UpdatePsychrometric(Psychrometric report)
    {
        var JobProgress = report.jobProgress;
        var arrayFilter = new [] {
            /* new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument("j.readingsType", new BsonDocument("$eq", JobProgress!.readingsType))
            ), */
            new BsonDocumentArrayFilterDefinition <BsonDocument> (
                new BsonDocument("j.date", new BsonDocument("$in", new BsonArray(new [] { JobProgress!.date }))
            ))
        };
        var dto = new PsychrometricDto()
        {
            JobId = report.JobId,
            ReportType = report.ReportType,
            formType = report.formType,
            jobProgress = new List<JobProgress>()
            {
                JobProgress
            }
        };
        await _psy.GenericFindOneUpdate<Psychrometric>(
            f => f.JobId == report.JobId && f.ReportType == report.ReportType && f.formType == report.formType,
            dto, upsert: false, action: "update", arrFilter: arrayFilter,
            setArrUpdate: Builders<PsychrometricDto>.Update.Set("jobProgress.$[j]", JobProgress));
        
        return CreatedAtAction(nameof(GetAll), "Psychrometric chart updated successfully!");
    }
    
    [HttpGet("{email}/employee")]
    public ActionResult<List<Report>> GetUserReports(string email)
    {
        var reports = _report.FilterBy(f => f.teamMember.email == email).ToList();
        return reports;
    }
}