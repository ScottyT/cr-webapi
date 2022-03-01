using System.Linq.Expressions;
using System.Text.Json;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace cr_app_webapi.Services
{
    public class ReportsService : IReportService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<AssignmentOfBenefits> _assignmentOfBenefits;
        private readonly IMongoCollection<CertificateOfCompletion> _certificate;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMongoCollection<CreditCard> _creditCard;
        private readonly IMongoCollection<CaseFile> _caseFile;
      
        public ReportsService(ICodeRedDatabaseSettings settings)
        {
            _database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _creditCard = _database.GetCollection<CreditCard>("credit-cards");
            _caseFile = _database.GetCollection<CaseFile>("reports");
            _certificate = _database.GetCollection<CertificateOfCompletion>("reports");
            _assignmentOfBenefits = _database.GetCollection<AssignmentOfBenefits>("reports");
            _contextAccessor = new HttpContextAccessor();
        }

        // Generics for report
        public class ReportLogic<T> where T : Report, new()
        {
            public async Task Create(IMongoCollection<T> collection, string report)
            {
                var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                T? c = JsonSerializer.Deserialize<T>(report);
                if (c is not null) 
                {
                    c.createdAt = time;
                    c.updatedAt = time;
                    await collection.InsertOneAsync(c);
                }
            }
        }

        public async Task CreateReport(string reportType, string report)
        {
            if (report is "" && reportType is "")
            {
                return;
            }
            ReportLogic<Dispatch> d = new ReportLogic<Dispatch>();
            ReportLogic<RapidResponse> rapid = new ReportLogic<RapidResponse>();
            ReportLogic<CaseFile> cf = new ReportLogic<CaseFile>();
            ReportLogic<Upholstery> u = new ReportLogic<Upholstery>();
            ReportLogic<ServiceAgreement> ser = new ReportLogic<ServiceAgreement>();
            ReportLogic<QualityControl> qual = new ReportLogic<QualityControl>();
            ReportLogic<CertificateOfCompletion> coc = new ReportLogic<CertificateOfCompletion>();
            ReportLogic<AssignmentOfBenefits> aob = new ReportLogic<AssignmentOfBenefits>();

            switch (reportType)
            {
                case "dispatch":
                    var dispatchCol = _database.GetCollection<Dispatch>("reports");
                    await d.Create(dispatchCol, report);
                    break;
                case "rapid-response":
                    var rapidCol = _database.GetCollection<RapidResponse>("reports");
                    await rapid.Create(rapidCol, report);
                    break;
                case "containment-sheet":
                case "tech-sheet":
                    var caseFileCol = _database.GetCollection<CaseFile>("reports");
                    await cf.Create(caseFileCol, report);
                    break;
                case "quality-control":
                    var qualityCol = _database.GetCollection<QualityControl>("reports");
                    await qual.Create(qualityCol, report);
                    break;
                case "upholstery-form":
                    var upholstery = _database.GetCollection<Upholstery>("reports");
                    await u.Create(upholstery, report);
                    break;
            }
            if (reportType.Contains("coc"))
            {
                await coc.Create(_certificate, report);
                return;
            }
            if (reportType.Contains("aob"))
            {
                await aob.Create(_assignmentOfBenefits, report);
                return;
            }
            if (reportType.Contains("contracting-agreement"))
            {
                var contractCol = _database.GetCollection<ServiceAgreement>("reports");
                await ser.Create(contractCol, report);
                return;
            }
        }

        public List<object> GetContract(string reportType, string id)
        {
            List<Object> returnList = new List<object>();
            if (reportType.Contains("coc") && reportType.Contains("aob"))
            {
                var q = from cert in _certificate.AsQueryable().AsEnumerable().Where(x => x.JobId == id && x.ReportType == reportType)
                                 join card in _creditCard.AsQueryable() on
                                 cert.card_id equals card.cardNumber
                                 select new Certificate(cert, card);
                if (q.Count() <= 0)
                {
                    var c = (from cert in _certificate.AsQueryable().AsEnumerable()
                        where cert.JobId == id 
                        where cert.ReportType == reportType
                        select new Certificate { Cert = cert });
                    returnList = c.Cast<object>().ToList();
                }
                else returnList = q.Cast<object>().ToList();
            }
            else if (reportType.Contains("aob"))
            {
                var q = (from aob in _assignmentOfBenefits.AsQueryable().AsEnumerable().Where(x => x.JobId == id && x.ReportType == reportType)
                                 join card in _creditCard.AsQueryable() on
                                 aob.cardNumber equals card.cardNumber
                                 select new Aob(aob, card));
                
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

        public async Task<Object?> GetReport(string id)
        {
            var objectid = new ObjectId(id);
            var reportCollection = _database.GetCollection<Report>("reports");
            var report = await reportCollection.Find(r => r.Id == id).As<Object>().FirstOrDefaultAsync();
            return report;
        }

        // Also used to create chart
        public async Task UpdatePsychrometricChart(string reportJson, Psychrometric report, string action)
        {
            var reportBson = _database.GetCollection<BsonDocument>("reports");
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            BsonDocument doc = BsonDocument.Parse(reportJson);
            Psychrometric? rep = JsonSerializer.Deserialize<Psychrometric>(reportJson);
            var jobProgress = doc.Elements.Where(el => el.Name == "jobProgress").FirstOrDefault();
            if (rep is null) return;
            var filters = (Builders<BsonDocument>.Filter.Eq("JobId", report.JobId) & Builders<BsonDocument>.Filter.Eq("ReportType", report.ReportType)
                & Builders<BsonDocument>.Filter.Eq("formType", report.formType));
            var existingReport = reportBson.Find(filters).FirstOrDefault();
            
            switch (action)
            {
                case "new":
                    UpdateDefinition<BsonDocument> updateBuilder = Builders<BsonDocument>.Update.Push(jobProgress.Name, jobProgress.Value);
                    var update = updateBuilder;
                    if (existingReport is null) update = updateBuilder.Set("updatedAt", time).Set("createdAt", time);
                    else update = updateBuilder.Set("updatedAt", time);
                    var options = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = true };
                    await reportBson.FindOneAndUpdateAsync(filters, update, options);
                    break;
                case "update":
                    update = Builders<BsonDocument>.Update.Set("jobProgress.$[el]", jobProgress.Value)
                        .Set("updatedAt", time);
                    var arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("el.readingsType", new BsonDocument("$eq", report.jobProgress?.readingsType))
                    );
                    var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };
                    options = new FindOneAndUpdateOptions<BsonDocument> { ArrayFilters = arrayFilters };
                    await reportBson.FindOneAndUpdateAsync(filters, update, options);
                    break;
                default:
                    await HandleError(_contextAccessor, "Please provide an action type");
                    break;
            }
        }

        public async Task UpdateReport(string reportType, string jobId)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            switch (reportType)
            {
                case "containment-sheet":
                case "tech-sheet":
                    var filter = Builders<CaseFile>.Filter.Eq(doc => doc.JobId, jobId) & Builders<CaseFile>.Filter.Eq(doc => doc.ReportType, reportType);
                    var update = Builders<CaseFile>.Update.Set(doc => doc.updatedAt, time);
                    await _caseFile.UpdateOneAsync(doc => doc.JobId == jobId && doc.ReportType == reportType, update);
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