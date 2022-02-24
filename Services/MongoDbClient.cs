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
        private IMongoCollection<CertificateOfCompletion> _certificate;
        private IMongoCollection<AssignmentOfBenefits> _assignmentOfBenefits;
        private IMongoCollection<CreditCard> _creditCard;
        
        public MongoDbClient(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
            _reportCollection = _database.GetCollection<Report>("reports");
            _certificate = _database.GetCollection<CertificateOfCompletion>("reports");
            _assignmentOfBenefits = _database.GetCollection<AssignmentOfBenefits>("reports");
            _creditCard = _database.GetCollection<CreditCard>("credit-cards");
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
            switch (reportType)
            {
                case "dispatch":
                    var dRep = JsonSerializer.Deserialize<Dispatch>(report);
                    if (dRep is null) break;
                    dRep.createdAt = time;
                    dRep.updatedAt = time;
                    await _database.GetCollection<Dispatch>("reports").InsertOneAsync(dRep);
                    break;
                case "rapid-response":
                    var rapidRep = JsonSerializer.Deserialize<RapidResponse>(report);
                    if (rapidRep is null) break;
                    rapidRep.createdAt = time;
                    rapidRep.updatedAt = time;
                    await _database.GetCollection<RapidResponse>("reports").InsertOneAsync(rapidRep);
                    break;
                case "containment-sheet":
                case "tech-sheet":
                    var caseFileRep = JsonSerializer.Deserialize<CaseFile>(report);
                    if (caseFileRep is null) break;
                    caseFileRep.createdAt = time;
                    caseFileRep.updatedAt = time;
                    await _database.GetCollection<CaseFile>("reports").InsertOneAsync(caseFileRep);
                    break;
                case "quality-control":
                    var qcRep = JsonSerializer.Deserialize<QualityControl>(report);
                    if (qcRep is null) break;
                    qcRep.createdAt = time;
                    qcRep.updatedAt = time;
                    await _database.GetCollection<QualityControl>("reports").InsertOneAsync(qcRep);
                    break;
                case "upholstery-form":
                    var upholstery = JsonSerializer.Deserialize<Upholstery>(report);
                    if (upholstery is null) break;
                    upholstery.createdAt = time;
                    upholstery.updatedAt = time;
                    await _database.GetCollection<Upholstery>("reports").InsertOneAsync(upholstery);
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
                await _database.GetCollection<ServiceAgreement>("reports").InsertOneAsync(serviceAgreement);
                return;
            }
        }

        public class ReportStore<T>
        {
            public T? Data {get; set;}
        }
        record ReportSingleObject<T>(T Data);
    }
}