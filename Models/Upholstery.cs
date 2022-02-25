using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

public class Upholstery : Report
{
    public string? Customer {get; set;}
    public string? Technician {get; set;}
    public string? address {get ;set;}
    public string? phoneNumber {get ;set;}
    public Object? groupedData {get ;set;}
    public string? ageOfFabric {get ;set;}
    public string? limitations {get ;set;}
    public Boolean techSig {get ;set;}
    public string? customerSig {get ;set;}
    public string? customerSignDate {get ;set;}
}