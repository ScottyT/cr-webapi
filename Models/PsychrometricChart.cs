using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
[BsonCollection("reports")]
public class Psychrometric : Report
{
    public JobProgress jobProgress {get; set;} = new JobProgress();
}


public class JobProgress
{
    public Info? info {get; set;}
    public string? date {get; set;}
    public string? color {get; set;}
    public string? readingsType {get; set;}
}


public class Info
{
    public double dewPoint {get; set;}
    public int dryBulbTemp {get; set;}
    public int humidityRatio {get; set;}
    public string? relativeHumidity {get; set;}
    public string? vaporPressure {get; set;}
}