using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using System.Dynamic;

namespace cr_app_webapi.Services
{
    public class CodeRedServices
    {
        private readonly IMongoCollection<Employee> _employeesCollection;
        private readonly IMongoCollection<Certification> _certification;
        private IMongoCollection<Report> _reportCollection;
        private IMongoCollection<BsonDocument> _repCollection;
        private IMongoCollection<Dispatch> _dispatch;
        private IMongoCollection<RapidResponse> _rapidResponse;
        private IMongoCollection<CaseFile> _caseFiles;
        private IMongoCollection<QualityControl> _qualityControl;
        private IMongoCollection<Upholstery> _upholstery;
        private IMongoCollection<ServiceAgreement> _serviceAgreement;
        private IMongoCollection<CertificateOfCompletion> _certificate;
        private IMongoCollection<AssignmentOfBenefits> _assignmentOfBenefits;
        private IMongoCollection<CreditCard> _creditCard;
        private IMongoCollection<Psychrometric> _psychrometric;
        private IMongoDatabase _database;
        private readonly IHttpContextAccessor _contextAccessor;
        public CodeRedServices(IOptions<CodeRedDatabaseSettings> settings, IHttpContextAccessor contextAccessor)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _contextAccessor = contextAccessor;
            _database = client.GetDatabase(settings.Value.DatabaseName);
            _employeesCollection = _database.GetCollection<Employee>("employees");
            _creditCard = _database.GetCollection<CreditCard>("credit-cards");
            _certification = _database.GetCollection<Certification>("certifications");
            _reportCollection = _database.GetCollection<Report>("reports");
            _repCollection = _database.GetCollection<BsonDocument>("reports");

            _dispatch = _database.GetCollection<Dispatch>("reports");
            _rapidResponse = _database.GetCollection<RapidResponse>("reports");
            _caseFiles = _database.GetCollection<CaseFile>("reports");
            _qualityControl = _database.GetCollection<QualityControl>("reports");
            _upholstery = _database.GetCollection<Upholstery>("reports");
            _serviceAgreement = _database.GetCollection<ServiceAgreement>("reports");
            _certificate = _database.GetCollection<CertificateOfCompletion>("reports");
            _assignmentOfBenefits = _database.GetCollection<AssignmentOfBenefits>("reports");
            _psychrometric = _database.GetCollection<Psychrometric>("reports");
        }

        public void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var exDict = expando as IDictionary<string, object>;
            if (exDict.ContainsKey(propertyName))
                exDict[propertyName] = propertyValue;
            else
            exDict.Add(propertyName, propertyValue);
        }

        // Employee section
        public async Task<List<Employee>> GetEmployees() => 
            await _employeesCollection.Find(_ => true).ToListAsync();

        public async Task<Employee?> GetEmployee(string teamid) =>
            await _employeesCollection.Find(x => x.id == teamid).FirstOrDefaultAsync();
        public async Task CreateEmployee(string emp)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var doc = JsonSerializer.Deserialize<Employee>(emp);
            if (doc is null) return;
            doc.createdAt = time;
            doc.updatedAt = time;
            await _employeesCollection.InsertOneAsync(doc);
        }
        public async Task CreateCertification(string certification)
        {
            var certBson = _database.GetCollection<BsonDocument>("certifications");
            BsonDocument doc = BsonDocument.Parse(certification);
            
            if (doc is null) return;
            await certBson.InsertOneAsync(doc);
        }

        public async Task<Certification?> GetCertification(string id) =>
            await _certification.Find(x => x._id == id).FirstOrDefaultAsync();
        public async Task<List<Certification>> GetCertifications() =>
            await _certification.Find(_ => true).ToListAsync();
        // Reports section
        public async Task<List<Report>> GetReports() =>
            await _reportCollection.Find(_ => true).ToListAsync();
        
        public async Task<List<Report>> UserReports(string email) =>
            await _reportCollection.Find(r => r.teamMember.email == email).ToListAsync();
        public async Task<Object?> GetReport(string id, string reportType)
        {
            var filters = (Builders<BsonDocument>.Filter.Eq("JobId", id) & Builders<BsonDocument>.Filter.Eq("ReportType", reportType));
            var ObjectsList = await _repCollection.Find(filters).FirstOrDefaultAsync();
            
            return ObjectsList.ToJson();
        }

        public List<Object> GetContract(string reportType, string id)
        {
            List<object> returnList = new List<object>();
            if (reportType.Contains("coc"))
            {
                var q = from cert in _certificate.AsQueryable().AsEnumerable().Where(x => x.JobId == id && x.ReportType == reportType)
                                 join card in _creditCard.AsQueryable() on
                                 cert.card_id equals card.cardNumber
                                 select new Certificate
                                 {
                                     Cert = cert,
                                     creditCard = card
                                 };
                if (q.Count() <= 0)
                {
                    var c = (from aob in _assignmentOfBenefits.AsQueryable().AsEnumerable()
                        where aob.JobId == id 
                        where aob.ReportType == reportType
                        select new Aob { AOB = aob });
                    returnList = c.Cast<object>().ToList();
                }
                else returnList = q.Cast<object>().ToList();
            }
            else if (reportType.Contains("aob"))
            {
                var q = (from aob in _assignmentOfBenefits.AsQueryable().AsEnumerable().Where(x => x.JobId == id && x.ReportType == reportType)
                                 join card in _creditCard.AsQueryable() on
                                 aob.cardNumber equals card.cardNumber
                                 select new Aob
                                 {
                                     AOB = aob,
                                     creditCard = card
                                 });
                
                if (q.Count() <= 0)
                {
                    var a = (from aob in _assignmentOfBenefits.AsQueryable().AsEnumerable()
                        where aob.JobId == id 
                        where aob.ReportType == reportType
                        select new Aob { AOB = aob });
                    returnList = a.Cast<object>().ToList();
                } 
                else returnList = q.Cast<object>().ToList();
            }
            return returnList;
        }

        public async Task CreateDispatch(Dispatch report) =>
            await _dispatch.InsertOneAsync(report);

        public async Task Create(string reportType, string report)
        {
            if (report is "" && reportType is "")
            {
                return;
            }
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            switch (reportType)
            {
                case "dispatch":
                    var dRep = JsonSerializer.Deserialize<Dispatch>(report);
                    if (dRep is null) break;
                    dRep.createdAt = time;
                    dRep.updatedAt = time;
                    await _dispatch.InsertOneAsync(dRep);
                    break;
                case "rapid-response":
                    var rapidRep = JsonSerializer.Deserialize<RapidResponse>(report);
                    if (rapidRep is null) break;
                    rapidRep.createdAt = time;
                    rapidRep.updatedAt = time;
                    await _rapidResponse.InsertOneAsync(rapidRep);
                    break;
                case "containment-sheet":
                case "tech-sheet":
                    var caseFileRep = JsonSerializer.Deserialize<CaseFile>(report);
                    if (caseFileRep is null) break;
                    caseFileRep.createdAt = time;
                    caseFileRep.updatedAt = time;
                    await _caseFiles.InsertOneAsync(caseFileRep);
                    break;
                case "quality-control":
                    var qcRep = JsonSerializer.Deserialize<QualityControl>(report);
                    if (qcRep is null) break;
                    qcRep.createdAt = time;
                    qcRep.updatedAt = time;
                    await _qualityControl.InsertOneAsync(qcRep);
                    break;
                case "upholstery-form":
                    var upholstery = JsonSerializer.Deserialize<Upholstery>(report);
                    if (upholstery is null) break;
                    upholstery.createdAt = time;
                    upholstery.updatedAt = time;
                    await _upholstery.InsertOneAsync(upholstery);
                    break;
            }
            if (reportType.Contains("coc"))
            {
                var coc = JsonSerializer.Deserialize<CertificateOfCompletion>(report);
                if (coc is null) return;
                coc.createdAt = time;
                coc.updatedAt = time;
                await _certificate.InsertOneAsync(coc);
                return;
            }
            if (reportType.Contains("aob"))
            {
                var aob = JsonSerializer.Deserialize<AssignmentOfBenefits>(report);
                if (aob is null) return;
                aob.createdAt = time;
                aob.updatedAt = time;
                await _assignmentOfBenefits.InsertOneAsync(aob);
                return;
            }
            if (reportType.Contains("contracting-agreement"))
            {
                var serviceAgreement = JsonSerializer.Deserialize<ServiceAgreement>(report);
                if (serviceAgreement is null) return;
                serviceAgreement.createdAt = time;
                serviceAgreement.updatedAt = time;
                await _serviceAgreement.InsertOneAsync(serviceAgreement);
                return;
            }
        }
        // Also used to create chart
        public async Task UpdatePsychrometricChart(string reportJson, Psychrometric report, string action)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            BsonDocument doc = BsonDocument.Parse(reportJson);
            Psychrometric? rep = JsonSerializer.Deserialize<Psychrometric>(reportJson);
            var jobProgress = doc.Elements.Where(el => el.Name == "jobProgress").FirstOrDefault();
            if (rep is null) return;
            var filters = (Builders<BsonDocument>.Filter.Eq("JobId", report.JobId) & Builders<BsonDocument>.Filter.Eq("ReportType", report.ReportType));
            var existingReport = _repCollection.Find(filters).FirstOrDefault();
            
            switch (action)
            {
                case "new":
                    UpdateDefinition<BsonDocument> updateBuilder = Builders<BsonDocument>.Update.Push(jobProgress.Name, jobProgress.Value);
                    var update = updateBuilder;
                    if (existingReport is null) update = updateBuilder.Set("updatedAt", time).Set("createdAt", time);
                    else update = updateBuilder.Set("updatedAt", time);
                    var options = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = true };
                    await _repCollection.FindOneAndUpdateAsync(filters, update, options);
                    break;
                case "update":
                    update = Builders<BsonDocument>.Update.Set("jobProgress.$[el]", jobProgress.Value)
                        .Set("updatedAt", time);
                    var arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("el.readingsType", new BsonDocument("$eq", report.jobProgress?.readingsType))
                    );
                    var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };
                    options = new FindOneAndUpdateOptions<BsonDocument> { ArrayFilters = arrayFilters };
                    await _repCollection.FindOneAndUpdateAsync(filters, update, options);
                    break;
                default:
                    await HandleError(_contextAccessor, "Please provide an action type");
                    break;
            }
        }
        public async Task Update(string reportType, string jobId)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            switch (reportType)
            {
                case "containment-sheet":
                case "tech-sheet":
                    //await _dispatch.ReplaceOneAsync(x => x.JobId == jobId && x.ReportType == reportType, (Dispatch)report);
                    
                    var update = Builders<Dispatch>.Update.Set("updatedAt", time);
                    await _dispatch.UpdateOneAsync(x => x.JobId == jobId && x.ReportType == reportType, update);
                    break;
            }
        }

        private async Task HandleError(IHttpContextAccessor context, string message)
        {
            var httpContext = context.HttpContext;
            if (httpContext is null) return;
            await httpContext.Response.WriteAsJsonAsync("Error: " + message);
        }
    }
}