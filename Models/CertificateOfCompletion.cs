using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;
[BsonIgnoreExtraElements]
public class CertificateOfCompletion : Report
{
    public string? subjectProperty {get; set;}
    public string? deductible {get; set;}
    public string? insuredMinEndDate {get; set;}

    [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
    public Dictionary<string, object>? insuredPayment1 {get; set;}

    [JsonConverter(typeof(DictionaryStringObjectJsonConverter))]
    public Dictionary<string, object>? insuredPayment2 {get; set;}
    public string? nonInsuredMinEndDate {get; set;}
    public string? nonInsuredPayment1 {get; set;}
    public string? nonInsuredPayment2 {get; set;}
    public string? rating {get; set;}
    public string? representative {get; set;}
    public string? repSignTime {get; set;}
    public string? representativeSign {get; set;}
    public string? repSignDate {get; set;}
    public Boolean teamSign {get; set;}
    public string? teamSignDate {get; set;}
    public string? testimonial {get; set;}
    public string? paymentOption {get; set;}
    public string? card_id {get; set;}
    [BsonIgnore]
    public CreditCard creditCard {get; set;} = new CreditCard();
}
public record struct Certificate(CertificateOfCompletion Cert, CreditCard creditCard);
/* {
    public CertificateOfCompletion Cert {get; set;} = new CertificateOfCompletion();
    public CreditCard creditCard {get; set;} = new CreditCard();
} */