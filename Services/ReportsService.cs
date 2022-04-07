using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using cr_app_webapi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace cr_app_webapi.Services
{
    public class ReportsService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<AssignmentOfBenefits> _assignmentOfBenefits;
        private readonly IMongoCollection<CertificateOfCompletion> _certificate;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMongoCollection<CreditCard> _creditCard;
        private readonly IMongoCollection<CaseFile> _caseFile;
        private readonly IMongoCollection<BsonDocument> _bsonCol;
        private readonly IMongoCollection<InventoryImage> _imagesBson;
        private readonly IMongoRepo<InventoryModel,InventoryModel> _image; //this might not work

        public ReportsService(ICodeRedDatabaseSettings settings, IMongoRepo<InventoryModel,InventoryModel> image)
        {
            _database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            _creditCard = _database.GetCollection<CreditCard>("credit-cards");
            _caseFile = _database.GetCollection<CaseFile>("reports");
            _certificate = _database.GetCollection<CertificateOfCompletion>("reports");
            _assignmentOfBenefits = _database.GetCollection<AssignmentOfBenefits>("reports");
            _contextAccessor = new HttpContextAccessor();
            _bsonCol = _database.GetCollection<BsonDocument>("reports");
            _imagesBson = _database.GetCollection<InventoryImage>("inventory.images");
            _image = image;
        }

        public async Task CreateReport(string reportType, string report)
        {
            if (report is "" && reportType is "")
            {
                return;
            }
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            var bsonConvert = BsonDocument.Parse(report);
            bsonConvert.Add("createdAt", time);
            bsonConvert.Add("updatedAt", time);
            await _bsonCol.InsertOneAsync(bsonConvert);
        }

        public async Task<Object?> GetReport(string id, string reportType)
        {
            var reportCollection = _database.GetCollection<Report>("reports");
            var projectionBuilder = Builders<Report>.Projection.Exclude(doc => doc.createdAt);
            var report = await reportCollection.Find(r => r.JobId == id && r.ReportType == reportType).As<Object>().FirstOrDefaultAsync();
            
            return report;
        }

        // Also used to create chart
        public async Task UpdatePsychrometricChart(string reportJson, Psychrometric report, string action)
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            BsonDocument doc = BsonDocument.Parse(reportJson);
            Psychrometric? rep = JsonSerializer.Deserialize<Psychrometric>(reportJson);
            var jobProgress = doc.Elements.Where(el => el.Name == "jobProgress").FirstOrDefault();
            if (rep is null) return;
            var filters = (Builders<BsonDocument>.Filter.Eq("JobId", report.JobId) & Builders<BsonDocument>.Filter.Eq("ReportType", report.ReportType)
                & Builders<BsonDocument>.Filter.Eq("formType", report.formType));
            var existingReport = _bsonCol.Find(filters).FirstOrDefault();

            switch (action)
            {
                case "new":
                    UpdateDefinition<BsonDocument> updateBuilder = Builders<BsonDocument>.Update.Push(jobProgress.Name, jobProgress.Value);
                    var update = updateBuilder;
                    if (existingReport is null) update = updateBuilder.Set("updatedAt", time).Set("createdAt", time);
                    else update = updateBuilder.Set("updatedAt", time);
                    var options = new FindOneAndUpdateOptions<BsonDocument> { IsUpsert = true };
                    await _bsonCol.FindOneAndUpdateAsync(filters, update, options);
                    break;
                case "update":
                    update = Builders<BsonDocument>.Update.Set("jobProgress.$[el]", jobProgress.Value)
                        .Set("updatedAt", time);
                    var arrayFilter = new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("el.readingsType", new BsonDocument("$eq", report.jobProgress?.readingsType))
                    );
                    var arrayFilters = new List<ArrayFilterDefinition> { arrayFilter };
                    options = new FindOneAndUpdateOptions<BsonDocument> { ArrayFilters = arrayFilters };
                    await _bsonCol.FindOneAndUpdateAsync(filters, update, options);
                    break;
                default:
                    await HandleError(_contextAccessor, "Please provide an action type");
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
    public static class ValidExtensions
    {
        public static bool IsValid<T>([NotNullWhen(true)]T? doc) =>
            doc is not null;
    }
}