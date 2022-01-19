using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string? Id {get; set;}
        public int? __v {get; set;}
        public string JobId {get; set;} = null!;
        public string ReportType {get; set;} = null!;
        public string formType {get; set;} = null!;
        public string appointmentDate {get; set;} = null!;
        public string appointmentTime {get; set;} = null!;
        public Object? callerName {get; set;}
        public string dateFormatted {get; set;} = null!;
        public string emailAddress {get; set;} = null!;
        public Object? location {get; set;}
        public string textTimeUpdate {get; set;} = null!;
        public string phoneNumber {get; set;} = null!;
        public string summary {get; set;} = null!;
        public Object? teamMember {get; set;}
        public string timeFormatted {get; set;} = null!;
        public string callTimeUpdate {get; set;} = null!;
        public Boolean teamMemberSig {get; set;}
        public string signDate {get; set;} = null!;
        public string DateOfLoss {get; set;} = null!;
        public string ClaimNumber {get; set;} = null!;
        public string ContactName {get; set;} = null!;
        public string DateOfEvaluation {get; set;} = null!;
        public List<Object> evaluationLogs {get; set;} = new List<Object>();
        public List<Object> documentVerification {get; set;} = new List<Object>();
        public string InsuranceCompany {get; set;} = null!;
        public List<Object> PictureTypes {get; set;} = new List<Object>();
        public string PolicyNumber {get; set;} = null!;
        public string adjusterName {get; set;} = null!;
        public string adjusterEmail {get; set;} = null!;
        public string adjusterPhone {get; set;} = null!;
        public string PropertyOwner {get; set;} = null!;
        public Object? ArrivalContactName {get; set;}
        public List<Object> propertyChkList {get; set;} = new List<Object>();
        public List<Object> intrusion {get; set;} = new List<Object>();
        public List<Object> Steps {get; set;} = new List<Object>();
        public List<Object> sourceWaterIntrusion {get; set;} = new List<Object>();
        public string cusFirstName {get; set;} = null!;
        public string cusLastName {get; set;} = null!;
        public string customerSig {get; set;} = null!;
        public string cusSignDate {get; set;} = null!;
        public string moistureMap {get; set;} = null!;
        public string id {get; set;} = null!;
        public string initial1 {get; set;} = null!;
        public string initial2 {get; set;} = null!;
        public string initial3 {get; set;} = null!;
        public string initial4 {get; set;} = null!;
        public string date {get; set;} = null!;
        public List<Object> selectedTmpRepairs {get; set;} = new List<Object>();
        public List<Object> selectedContent {get; set;} = new List<Object>();
        public List<Object> selectedStructualCleaning {get; set;} = new List<Object>();
        public List<Object> selectedStructualDrying {get; set;} = new List<Object>();
        public List<Object> selectedCleaningSection {get; set;} = new List<Object>();
        public List<Object> contentCleaningInspection {get; set;} = new List<Object>();
        public List<Object> waterRestorationInspection {get; set;} = new List<Object>();
        public List<Object> waterRemediationAssesment {get; set;} = new List<Object>();
        public List<Object> overviewScopeOfWork {get; set;} = new List<Object>();
        public List<Object> specializedExpert {get; set;} = new List<Object>();
        public List<Object> scopeOfWork {get; set;} = new List<Object>();
        public List<Object> projectWorkPlans {get; set;} = new List<Object>();
        public string notes {get; set;} = null!;
        public Boolean verifySign {get; set;}
        public string afterHoursWork {get; set;} = null!;
        public List<Object> serviceArr {get; set;} = new List<Object>();
        public string numberOfDehus {get; set;} = null!;
        public string waterGallons {get; set;} = null!;
        public string waterPounds {get; set;} = null!;
        public Boolean techSig {get; set;}
        public string? sketch {get; set;}
        public string? Customer {get; set;}
        public string? Technician {get; set;}
        public List<object> categoryData {get; set;} = new List<object>();
    }
}