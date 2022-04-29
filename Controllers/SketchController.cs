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

    [HttpGet("{id:length(24)}")]
    public ActionResult<SketchDTO> Get(string id)
    {
        var sketch = _sketchRepo.FilterBy(
            filter => filter.Id == id,
            p => new SketchDTO
            {
                JobId = p.JobId,
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