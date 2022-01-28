using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace cr_app_webapi.Models
{
    [BsonIgnoreExtraElements]
    public class Report
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id {get; set;}
        /* [BsonElement("ts")]
        [BsonRepresentation(BsonType.Timestamp)]
        public DateTime Timestamp {get; set;} */
        public string JobId {get; set;} = null!;
        public string? ReportType {get; set;}
        public string? formType {get; set;}
        public TeamMember teamMember {get; set;} = new TeamMember();
        public string? date {get; set;}
        /* public string? timeFormatted {get; set;}
        public string? callTimeUpdate {get; set;}
        public Boolean teamMemberSig {get; set;}
        public string? signDate {get; set;}
        public string? DateOfLoss {get; set;}
        public string? ClaimNumber {get; set;}
        public Object? ContactName {get; set;}
        public string? DateOfEvaluation {get; set;}
        public List<Object> evaluationLogs {get; set;} = new List<Object>();
        public List<Object> documentVerification {get; set;} = new List<Object>();
        public string? InsuranceCompany {get; set;}
        public List<Object> PictureTypes {get; set;} = new List<Object>();
        public string? PolicyNumber {get; set;}
        public string? adjusterName {get; set;}
        public string? adjusterEmail {get; set;}
        public string? adjusterPhone {get; set;}
        public Object? PropertyOwner {get; set;}
        public Object? ArrivalContactName {get; set;}
        public List<string> propertyChkList {get; set;} = new List<string>();
        public List<Object> intrusion {get; set;} = new List<Object>();
        public List<Object> Steps {get; set;} = new List<Object>();
        public List<Object> sourceWaterIntrusion {get; set;} = new List<Object>();
        public string? cusFirstName {get; set;}
        public string? cusLastName {get; set;}
        public string? customerSig {get; set;}
        public string? cusSignDate {get; set;}
        public string? moistureMap {get; set;}
        public string? id {get; set;} = null!;
        public string? initial1 {get; set;}
        public string? initial2 {get; set;}
        public string? initial3 {get; set;}
        public string? initial4 {get; set;}
        
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
        public string? notes {get; set;}
        public Boolean verifySign {get; set;}
        public string? afterHoursWork {get; set;}
        public List<Object> serviceArr {get; set;} = new List<Object>();
        public string? numberOfDehus {get; set;}
        public string? waterGallons {get; set;}
        public string? waterPounds {get; set;}
        public Boolean techSig {get; set;}
        public string? sketch {get; set;}
        public string? Customer {get; set;}
        public string? Technician {get; set;}
        public List<object> categoryData {get; set;} = new List<object>();
        public Object? groupedData {get; set;} */
    }
}