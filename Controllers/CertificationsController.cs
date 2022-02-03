using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace cr_app_webapi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificationsController : ControllerBase
{
    private readonly CodeRedServices _certificationService;
    public CertificationsController(CodeRedServices certificationService)
    {
        _certificationService = certificationService;
    }
    
    [HttpGet]
    public async Task<List<Certification>> Get() =>
        await _certificationService.GetCertifications();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Certification>> Get(string id)
    {
        var cert = await _certificationService.GetCertification(id);
        if (cert is null)
        {
            return NotFound();
        }
        return cert;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Certification cert)
    {
        var jsonstring = JsonSerializer.Serialize(cert);
        await _certificationService.CreateCertification(jsonstring);
        return CreatedAtAction(nameof(Get), new { _id = cert._id }, "Successfully created certification!");
    }
}