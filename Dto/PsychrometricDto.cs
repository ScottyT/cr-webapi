using cr_app_webapi.Models;

namespace cr_app_webapi.Dto;

[BsonCollection("reports")]
public class PsychrometricDto : Document
{
    public PsychrometricDto()
    {
        createdAt = DateTime.UtcNow;
        updatedAt = DateTime.UtcNow;
    }
    public string? JobId {get; set;}
    public string? ReportType {get; set;}
    public string? formType {get; set;}
    public TeamMember teamMember {get; set;} = new TeamMember();
    public List<JobProgress> jobProgress {get; set;} = new List<JobProgress>();
}