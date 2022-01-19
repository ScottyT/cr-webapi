using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace cr_app_webapi.Services
{
    public class CodeRedServices
    {
        private readonly IMongoCollection<Employee> _employeesCollection;
        private readonly IMongoCollection<Report> _reportCollection;
        public CodeRedServices(IOptions<CodeRedDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _employeesCollection = database.GetCollection<Employee>("employees");
            _reportCollection = database.GetCollection<Report>("reports");
        }

        public async Task<List<Employee>> GetEmployeesAsync() => 
            await _employeesCollection.Find(_ => true).ToListAsync();
        public async Task<List<Report>> GetReportsAsync() =>
            await _reportCollection.Find(_ => true).ToListAsync();
        public List<Report> GetReports() => _reportCollection.Find(report => true).ToList();
    }
}