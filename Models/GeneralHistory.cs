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
    public string? ReportType { get{ return "GeneralHistory"; } }
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
    public string? emailSentDate {get; set;}
}

public class PaymentArray
{
    public string? paymentType {get; set;}
    public string? amount {get; set;}
    public string? date {get; set;}
    public string? keyNotes {get; set;}
}