using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificationsController : ControllerBase
{
    private readonly IMongoRepo<Certification,Certification> _certification;
    public CertificationsController(IMongoRepo<Certification,Certification> certification)
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

    [HttpGet("details/{auth_id}")]
    public ActionResult<List<Certification>> GetByUser(string auth_id)
    {
        var cert = _certification.FilterBy(
            f => f.teamMemberAuthId == auth_id
        ).ToList();
        return cert;
    }

    [HttpPost("create")]
    public IActionResult Post(Certification cert)
    {
        var newCert = _certification.InsertOneAsync(
            filter => filter.Id == cert.Id, cert
        );
        return CreatedAtAction(nameof(Get), new { _id = cert.Id }, new { message = "Successfully created certification!", result = newCert});
    }
}