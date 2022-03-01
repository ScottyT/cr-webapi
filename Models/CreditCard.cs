using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
[BsonCollection("credit-cards")]
public class CreditCard : Document
{
    public string? ReportType {get; set;}
    public Object? cardholderInfo {get; set;}
    public Object? billingAddressFirst {get; set;}
    public Object? billingAddressOther {get; set;}
    public string? creditCardCompany {get; set;}
    public string? cardholderName {get; set;}
    public string? expirationDate {get; set;}
    public string? cvvNum {get; set;}
    public string? cardholderZip {get; set;}
    public string? cardNumber {get; set;}
    public string? customerSig {get; set;}
    public string? customerSignDate {get; set;}
    public string? teamMember {get; set;}
}