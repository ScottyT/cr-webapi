using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
public class AssignmentOfBenefits : Report
{
    public string? contractingCompany {get; set;}
    public string? subjectProperty {get; set;}
    public string? cusSign1 {get; set;}
    public string? cusSignDate1 {get; set;}
    public string? initial1 {get; set;}
    public string? initial2 {get; set;}
    public string? initial3 {get; set;}
    public string? initial4 {get; set;}
    public string? initial5 {get; set;}
    public string? initial6 {get; set;}
    public string? initial7 {get; set;}
    public string? initial8 {get; set;}
    public string? insuredTermEndDate {get; set;}
    public string? insuredPay1 {get; set;}
    public string? insuredPayDay1 {get; set;}
    public string? insuredPay2 {get; set;}
    public string? insuredPayDay5 {get; set;}
    public string? nonInsuredTermEndDate {get; set;}
    public string? nonInsuredDay1 {get; set;}
    public string? nonInsuredDay5 {get; set;}
    public Object? location {get; set;}
    public string? firstName {get; set;}
    public string? lastName {get; set;}
    public string? email {get; set;}
    public string? driversLicense {get; set;}
    public string? relation {get; set;}
    public string? minimumSqft {get; set;}
    public string? representativePrint {get; set;}
    public string? repSignature {get; set;}
    public string? propertyRepOf {get; set;}
    public string? repDateSign {get; set;}
    public string? witness {get; set;}
    public string? witnessDate {get; set;}
    public string? numberOfRooms {get; set;}
    public string? numberOfFloors {get; set;}
    public string? paymentOption {get; set;}
    public string? cusSign2 {get; set;}
    public string? cusSignDate2 {get; set;}
    [BsonElement("cardNumber")]
    public string? cardNumber {get; set;}
    [BsonIgnore]
    public CreditCard? creditCard {get; set;} = new CreditCard();
}

[BsonIgnoreExtraElements]
public record struct Aob(AssignmentOfBenefits? AOB, CreditCard creditCard);
/* {
    public AssignmentOfBenefits? AOB {get; set;} = new AssignmentOfBenefits();
    public CreditCard creditCard {get; set;} = new CreditCard();
} */