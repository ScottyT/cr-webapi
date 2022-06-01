using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Dto;

[BsonIgnoreExtraElements]
public class GeneralHistoryDTO
{
    /* BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;} */
    public string JobId {get; set;} = null!;
    public string? ReportType {get; set;}
    public string? customerFirstName {get; set;}
    public string? customerLastName {get; set;}
    public string? customerPhoneNumber {get; set;}
    public string? customerEmail {get; set;}
    public Location location {get; set;} = new Location();
    public string? insuranceCompany {get; set;}
    public string? adjusterName {get; set;}
    public string? adjusterPhoneNumber {get; set;}
    public string? adjusterEmail {get; set;}
    public string? claimNumber {get; set;}
    public string? policyNumber {get; set;}
    public List<PaymentArray> payments {get; set;} = new List<PaymentArray>();
    public string? startOfJob {get; set;}
    public string? endOfJob {get; set;}
    public string? initialEmailSentDate {get; set;}
    public List<LogsArray> logs {get; set;} = new List<LogsArray>();
}