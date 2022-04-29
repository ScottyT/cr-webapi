using cr_app_webapi.Models;

namespace cr_app_webapi.Dto;

public class SketchDTO
{
    public string JobId { get; set; } = null!;
    public string ReportType { get; set; } = default!;
    public string? formType { get; set; }
    public TeamMember teamMember { get; set; } = new TeamMember();
    public string? sketch {get; set;}
    public string? notes {get; set;}
}