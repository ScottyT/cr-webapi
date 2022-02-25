using cr_app_webapi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;

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
        public CodeRedServices(IOptions<CodeRedDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
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
        // ------- End Employee Section ----- //

        // Reports section //


    }
}