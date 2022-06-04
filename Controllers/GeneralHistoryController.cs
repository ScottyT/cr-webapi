using cr_app_webapi.Dto;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("read:reports")]
public class GeneralHistoryController : ControllerBase
{
    private readonly IMongoRepo<GeneralHistory, GeneralHistory> _generalHistory;
    public GeneralHistoryController(IMongoRepo<GeneralHistory, GeneralHistory> generalHistory)
    {
        _generalHistory = generalHistory;
    }

    [HttpGet]
    public IEnumerable<GeneralHistory> GetAll()
    {
        return _generalHistory.AsQueryable().ToList();
    }

    [HttpGet("{jobid}")]
    public ActionResult<Object> Get(string jobid)
    {
        var log = _generalHistory.FilterBy<GeneralHistoryDTO>(
            filter => filter.JobId == jobid,
            project => new GeneralHistoryDTO()
            {
                payments = project.payments,
                startOfJob = project.startOfJob,
                endOfJob = project.endOfJob,
                initialEmailSentDate = project.initialEmailSentDate,
                logs = project.logs
            }
        ).FirstOrDefault();
        if (log is null)
        {
            return NotFound();
        }
        return log;
    }

    [HttpPost("new")]
    public async Task<IActionResult> Create(GeneralHistory postData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _generalHistory.InsertOneAsync(postData);

        return CreatedAtAction(nameof(GetAll), new { id = postData.Id }, "New General History log created!");
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(GeneralHistory updateData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _generalHistory.GenericFindOneUpdate<GeneralHistory>(
            filter => filter.JobId == updateData.JobId, updateData, upsert: true
        );
        return NoContent();
    }
}