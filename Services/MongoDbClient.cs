using System.Text.Json;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace cr_app_webapi.Services
{
    public class MongoDbClient : IMongoDbClient
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Report> _reportCollection;
        private readonly IMongoCollection<CertificateOfCompletion> _certificate;
        private readonly IMongoCollection<Dispatch> _dispatch;
        private readonly IMongoCollection<AssignmentOfBenefits> _assignmentOfBenefits;
        private readonly IMongoCollection<CreditCard> _creditCard;
        private readonly IMongoCollection<Employee> _employeesCollection;
        private readonly IHttpContextAccessor _contextAccessor;
        
        
        public MongoDbClient(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
            _reportCollection = _database.GetCollection<Report>("reports");
            _certificate = _database.GetCollection<CertificateOfCompletion>("reports");
            _assignmentOfBenefits = _database.GetCollection<AssignmentOfBenefits>("reports");
            _dispatch = _database.GetCollection<Dispatch>("reports");
            _creditCard = _database.GetCollection<CreditCard>("credit-cards");
            _employeesCollection = _database.GetCollection<Employee>("employees");
            _contextAccessor = new HttpContextAccessor();
        }

        public class ReportStore<T>
        {
            public T? Data {get; set;}
        }
        class ReportLogic<T> where T : Report, new()
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

        public async Task<Object> GetReport(string reportid)
        {
            ReportStore<Object> store = new ReportStore<Object>();
            var reportModelCol = _database.GetCollection<Report>("reports");
            store.Data = await reportModelCol.Find(x => x.Id == reportid).As<Object>().FirstOrDefaultAsync();
            return store.Data;
        }

        public async Task<List<Report>> GetReports()
        {
            var data = await _reportCollection.Find(_ => true).As<Report>().ToListAsync();
            return data;
        }

        public async Task<List<Report>> UserReports(string email) =>
            await _reportCollection.Find(r => r.teamMember.email == email).ToListAsync();

        public List<Object> GetContract(string reportType, string id)
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

        public async Task CreateReport(string reportType, string report)
        {
            if (report is "" && reportType is "")
            {
                return;
            }
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
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
                    ReportStore<CaseFile> caseFile = new ReportStore<CaseFile>();
                    var caseFileCol = _database.GetCollection<ReportStore<CaseFile>>("reports");
                    var update = Builders<ReportStore<CaseFile>>.Update.Set("updatedAt", time);
                    caseFile.Data = await caseFileCol.UpdateOneAsync<ReportStore<CaseFile>>(x => x.Data.JobId == jobId && x.Data.ReportType == reportType, update);
                    //await _dispatch.UpdateOneAsync(x => x.JobId == jobId && x.ReportType == reportType, update);
                    break;
            }
        }
        public async Task<Employee> GetUser(string email)
        {
            var e = await _employeesCollection.Find(e => e.email == email).FirstOrDefaultAsync();
            var user = new Employee
            {
                Id = e.Id,
                certifications_id = e.certifications_id,
                createdAt = e.createdAt,
                email = e.email,
                fullName = new FullName(e.fname, e.lname),
                role = e.role,
                id = e.id
            };
            return user;
        }

        public async Task<List<Employee>> GetUsers() =>
            await _employeesCollection.Find(_ => true).ToListAsync();

        public async Task CreateUser(Employee newUser)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            if (newUser is null) return;
            newUser.createdAt = time;
            newUser.updatedAt = time;
            await _employeesCollection.InsertOneAsync(newUser);
        }
        private async Task HandleError(IHttpContextAccessor context, string message)
        {
            var httpContext = context.HttpContext;
            if (httpContext is null) return;
            await httpContext.Response.WriteAsJsonAsync("Error: " + message);
        }
    }
}