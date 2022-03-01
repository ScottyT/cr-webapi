using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificationsController : ControllerBase
{
    private readonly IMongoRepo<Certification> _certification;
    public CertificationsController(IMongoRepo<Certification> certification)
    {
        _certification = certification;
    }
    
    [HttpGet]
    public List<Certification> Get() =>
        _certification.AsQueryable().ToList();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Certification>> Get(string id)
    {
        var cert = await _certification.GetOneAsync(id);
        if (cert is null)
        {
            return NotFound();
        }
        return cert;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Post(Certification cert)
    {
        var jsonstring = JsonSerializer.Serialize(cert);
        await _certification.SaveOneAsync(jsonstring);
        return CreatedAtAction(nameof(Get), new { _id = cert.Id }, "Successfully created certification!");
    }
}