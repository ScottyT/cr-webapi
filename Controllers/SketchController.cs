using cr_app_webapi.Dto;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize("read:reports")]
public class SketchController : ControllerBase
{
    private readonly IMongoRepo<Sketch, Sketch> _sketchRepo;

    public SketchController(IMongoRepo<Sketch, Sketch> sketchRepo)
    {
        _sketchRepo = sketchRepo;
    }

    [HttpGet]
    public List<Sketch> Get() =>
        _sketchRepo.AsQueryable().ToList();

    [HttpGet("{jobid}")]
    public IEnumerable<Sketch> GetByJobId(string jobid)
    {
        var sketches = _sketchRepo.FilterBy(
            filter => filter.JobId == jobid,
            p => new Sketch
            {
                JobId = p.JobId,
                Title = p.Title,
                Id = p.Id,
                ReportType = p.ReportType,
                sketch = p.sketch,
                notes = p.notes
            }
        );
        return (IEnumerable<Sketch>)sketches;
    }

    [HttpGet("{id:length(24)}")]
    public ActionResult<SketchDTO> Get(string id)
    {
        var sketch = _sketchRepo.FilterBy(
            filter => filter.Id == id,
            p => new SketchDTO
            {
                JobId = p.JobId,
                Title = p.Title,
                teamMember = p.teamMember,
                sketch = p.sketch,
                formType = p.formType,
                ReportType = p.ReportType,
                notes = p.notes
            }
        ).FirstOrDefault();

        if (sketch is null)
        {
            return NotFound();
        }
        return sketch;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Sketch newSketch)
    {
        await _sketchRepo.InsertOneAsync(newSketch);
        return CreatedAtAction(nameof(Get), new { id = newSketch.Id }, "Successfully created Sketch!");
    }
}