using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
[BsonCollection("credit-cards")]
public class CreditCard : Document
{
    public CreditCard()
    {
        createdAt = DateTime.Now;
        updatedAt = DateTime.Now;
    }
    public string? ReportType {get; set;}

    public CardholderInfo? cardholderInfo {get; set;}
    public Location billingAddressFirst {get; set;} = new Location();
    public Location billingAddressOther {get; set;} = new Location();
    public string? creditCardCompany {get; set;}
    public string? cardholderName {get; set;}
    public string? expirationDate {get; set;}
    public string? cvvNum {get; set;}
    public string? cardholderZip {get; set;}

   // [ForeignKey("cardNumber")]
    public string cardNumber {get; set;} = default!;
    public string? customerSig {get; set;}
    public string? customerSignDate {get; set;}
}

public class CardholderInfo
{
    public string? first {get; set;}
    public string? middle {get; set;}
    public string? last {get; set;}
    public string? email {get; set;}
    public string? phoneNumber {get; set;}
}