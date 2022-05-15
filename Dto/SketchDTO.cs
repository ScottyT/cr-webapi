using cr_app_webapi.Models;

namespace cr_app_webapi.Dto;

public class SketchDTO
{
    public string JobId { get; set; } = null!;
    public string? title {get; set;}
    public string ReportType { get; set; } = default!;
    public string? formType { get; set; }
    public string? sketch {get; set;}
    public string? notes {get; set;}
}