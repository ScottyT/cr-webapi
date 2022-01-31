using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

public class Upholstery
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id {get; set;}
    public string? JobId {get; set;}
    public string? Customer {get; set;}
    public string? Technician {get; set;}
    public string? address {get ;set;}
    public string? phoneNumber {get ;set;}
    public string? date {get ;set;}
    public string? ReportType {get ;set;}
    public string? formType {get ;set;}
    public Object? teamMember {get ;set;}
    public Object? groupedData {get ;set;}
    public string? ageOfFabric {get ;set;}
    public string? limitations {get ;set;}
    public Boolean techSig {get ;set;}
    public string? customerSig {get ;set;}
    public string? customerSignDate {get ;set;}
    public DateTime createdAt {get; set;}
    public DateTime updatedAt {get; set;}
}