using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace cr_app_webapi.Models;

[BsonIgnoreExtraElements]
public class RapidResponse : Report
{
    public string? DateOfLoss {get; set;}
    public string? ClaimNumber {get; set;}
    public Object? ContactName {get; set;}
    public string? DateOfEvaluation {get; set;}
    public string? emailAddress {get; set;}
    public List<object> evaluationLogs {get; set;} = new List<object>();
    public List<string> documentVerification {get; set;} = new List<string>();
    public string? InsuranceCompany {get; set;}
    public string? phoneNumber {get; set;}
    public List<object> PictureTypes {get; set;} = new List<object>();
    public string? PolicyNumber {get; set;}
    public string? adjusterName {get; set;}
    public string? adjusterEmail {get; set;}
    public string? adjusterPhone {get; set;}
    public Object? PropertyOwner {get; set;}
    public List<object> Steps {get; set;} = new List<object>();
    public List<object> sourceWaterIntrusion {get; set;} = new List<object>();
    public string? cusFirstName {get; set;}
    public string?cusLastName {get; set;}
    public string? customerSig {get; set;}
    public string? cusSignDate {get; set;}
    public string? moistureMap {get; set;}
    [BsonElement("id")]
    public string? team_id {get; set;}
    public Object? location {get; set;}
    public string? signDate {get; set;}
    public List<object> intrusion {get; set;} = new List<object>();
    public string? dateIntrusion {get; set;}
    public string? timeIntrusion {get; set;}
    public Dictionary<int, string> preliminaryDetermination {get; set;} = new Dictionary<int, string>();
    public Dictionary<string, string> moistureInspection {get; set;} = new Dictionary<string, string>();
    public Object? preRestorationEval {get; set;}
    public Boolean teamMemberSig {get; set;}
    public string? initial1 {get; set;}
    public string? initial2 {get; set;}
    public string? initial3 {get; set;}
    public string? initial4 {get; set;}
}