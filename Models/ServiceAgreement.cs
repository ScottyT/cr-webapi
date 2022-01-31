using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

public class ServiceAgreement
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;}
    public string? JobId {get; set;}
    public string? ReportType {get; set;}
    public string? formType {get; set;}
    public string? contractingCompany {get; set;}
    public Object? teamMember {get; set;}
    public string? jobName {get; set;}
    public Object? jobLocation {get; set;}
    public string? cityStateZip {get; set;}
    public string? contactLiaison {get; set;}
    public string? contactPhone {get; set;}
    public string? contactEmail {get; set;}
    public string? emergencyLiaison {get; set;}
    public string? emergencyPhone {get; set;}
    public Object? billingLocation {get; set;}
    public string? faxNumber {get; set;}
    public string? mortgageCompany {get; set;}
    public string? insuranceCompany {get; set;}
    public string? dateOfLoss {get; set;}
    public string? claimNumber {get; set;}
    public string? policyNumber {get; set;}
    public string? policyDate {get; set;}
    public string? customerPrint {get; set;}
    public string? cusSign {get; set;}
    public string? signDate {get; set;}
    public DateTime createdAt {get; set;}
    public DateTime updatedAt {get; set;}
}