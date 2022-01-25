using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace cr_app_webapi.Models;

public class CaseFile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;}
    public string JobId {get; set;} = null!;
    public string? ReportType {get; set;}
    public string? formType {get; set;}
    public string? date {get; set;}
    public string? id {get; set;}
    public Object? location {get; set;}
    public Dictionary<int, string> selectedTmpRepairs {get; set;} = new Dictionary<int, string>();
    public Dictionary<int, object> selectedContent {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> selectedStructualCleaning {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> selectedStructualDrying {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> selectedCleaningSection {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> contentCleaningInspection {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> waterRestorationInspection {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> waterRemediationAssesment {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> overviewScopeOfWork {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> specializedExpert {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> scopeOfWork {get; set;} = new Dictionary<int, object>();
    public Dictionary<int, object> projectWorkPlans {get; set;} = new Dictionary<int, object>();
    public string? notes {get; set;} 
    public Object? teamMember {get; set;} 
    public string? afterHoursWork {get; set;} 
    public List<object> evaluationLogs {get; set;} = new List<object>();
    public Boolean verifySign {get; set;} 
    public string? numberOfDehus {get; set;}
    public string? waterGallons {get; set;} 
    public string? waterPounds {get; set;} 
    public DateTime createdAt {get; set;}
    public DateTime updatedAt {get; set;}
}