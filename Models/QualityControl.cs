using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

public class QualityControl : Report
{
    public Object? location {get; set;}
    public List<string> completedDocs {get; set;} = new List<string>();
    public Object? completedServices {get; set;}
    public List<string> taskList {get; set;} = new List<string>();
    public List<string> customerReview {get; set;} = new List<string>();
    public string? evalTime {get; set;}
    public string? evalDate {get; set;}
    public Object? customer {get; set;}
    public string? customerSig {get; set;}
    public string? signDate {get; set;}
}