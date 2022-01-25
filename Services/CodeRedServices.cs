using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace cr_app_webapi.Services
{
    public class CodeRedServices
    {
        private readonly IMongoCollection<Employee> _employeesCollection;
        private IMongoCollection<Report> _reportCollection;
        private IMongoCollection<BsonDocument> _repCollection;
        private IMongoCollection<Dispatch> _dispatch;
        private IMongoCollection<RapidResponse> _rapidResponse;
        private IMongoCollection<CaseFile> _caseFiles;
        private IMongoDatabase _database;
        public CodeRedServices(IOptions<CodeRedDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            _employeesCollection = _database.GetCollection<Employee>("employees");
            _reportCollection = _database.GetCollection<Report>("reports");
            _repCollection = _database.GetCollection<BsonDocument>("reports");

            _dispatch = _database.GetCollection<Dispatch>("reports");
            _rapidResponse = _database.GetCollection<RapidResponse>("reports");
            _caseFiles = _database.GetCollection<CaseFile>("reports");
        }

        public async Task<List<Employee>> GetEmployees() => 
            await _employeesCollection.Find(_ => true).ToListAsync();
        public async Task<List<Report>> GetReports() =>
            await _reportCollection.Find(_ => true).ToListAsync();

        /* public async Task<List<Object>> Get()
        {
            var rep = await GetReports();
            var dispatch = await _dispatchCollection.Find(_ => true).ToListAsync();
            var emp = await GetEmployees();
            var arge = _reportCollection.Aggregate()
                .Lookup("reports", "_id", "_id", "Reports").Lookup("employees", "_id", "teamMember", "Employees");
            //List<object> result = rep.Cast<object>().Concat(dispatch).ToList();
            List<object> result = (from x in rep select(object)x).ToList();
            result.AddRange((from x in dispatch select (object)x).ToList());
            result.AddRange((from x in emp select (object)x).ToList());
            
            return result;
        } */
        
        public async Task<Object?> GetReport(string id, string reportType)
        {
            var filters = (Builders<BsonDocument>.Filter.Eq("JobId", id) & Builders<BsonDocument>.Filter.Eq("ReportType", reportType));
            var ObjectsList = await _repCollection.Find(filters).FirstOrDefaultAsync();
            /* switch (reportType) 
            {
                case "dispatch":
                    report = await _reportCollection.Find(x => x.JobId == id && x.ReportType == reportType).FirstOrDefaultAsync();
                    break;
            } */
            //ObjectsList.Select(v => (BsonValue)(ObjectId)v).ToList();
            return ObjectsList.ToJson();
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
    }
}