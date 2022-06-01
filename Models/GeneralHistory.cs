using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
[BsonCollection("general-history")]
public class GeneralHistory : Document
{
    public GeneralHistory()
    {
        createdAt = DateTime.UtcNow;
        updatedAt = DateTime.UtcNow;
    }
    public string? JobId {get; set;}
    public string? ReportType { get; set; } = "GeneralHistory";
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
public class PaymentArray
{
    public string? paymentType {get; set;}
    public double? amount {get ;set;}
    public string? date {get; set;}
    public string? keyNotes {get; set;}
}
public class LogsArray
{
    public string? date {get; set;}
    public string? time {get; set;}
    public string? typeOfCommunication {get; set;}
    public string? codeRedRepsName {get; set;}
    public string? communicationWith {get; set;}
    public string? summaryNotes {get; set;}
    public string? communicationRecords {get; set;}
}