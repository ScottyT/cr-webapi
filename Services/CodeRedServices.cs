using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using System.Text.Json;

namespace cr_app_webapi.Services
{
    public class CodeRedServices
    {
        private readonly IMongoCollection<Employee> _employeesCollection;
        private IMongoCollection<Report> _reportCollection;
        private IMongoCollection<Dispatch> _dispatchCollection;
        private IMongoDatabase _database;
        public CodeRedServices(IOptions<CodeRedDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
            _employeesCollection = _database.GetCollection<Employee>("employees");
            _reportCollection = _database.GetCollection<Report>("reports");
            _dispatchCollection = _database.GetCollection<Dispatch>("reports");
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
            Object report = new object();
            switch (reportType) 
            {
                case "dispatch":
                    report = await _dispatchCollection.Find(x => x.JobId == id && x.ReportType == reportType).FirstOrDefaultAsync();
                    break;
            }
            return report;
        }

        public async Task CreateDispatch(Dispatch report) =>
            await _dispatchCollection.InsertOneAsync(report);

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
                    if (dRep is null)
                    {
                        break;
                    }
                    dRep.createdAt = time;
                    dRep.updatedAt = time;
                    await _dispatchCollection.InsertOneAsync(dRep);
                    break;
                case "rapid-response":
                    
                    break;
            }
        }
        public async Task Update(string reportType, string jobId)
        {
            switch (reportType)
            {
                case "dispatch":
                    //await _dispatchCollection.ReplaceOneAsync(x => x.JobId == jobId && x.ReportType == reportType, (Dispatch)report);
                    var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    var update = Builders<Dispatch>.Update.Set("updatedAt", time);
                    await _dispatchCollection.UpdateOneAsync(x => x.JobId == jobId && x.ReportType == reportType, update);
                    break;
            }
        }
    }
}