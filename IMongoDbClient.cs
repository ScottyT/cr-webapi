using cr_app_webapi.Models;

namespace cr_app_webapi
{
    public interface IMongoDbClient
    {
        Task<List<Report>> GetReports();
        Task<Object> GetReport(string reportid);
        Task<List<Report>> UserReports(string email);
        List<Object> GetContract(string reportType, string id);
        Task CreateReport(string reportType, string report);
        Task<Employee> GetUser(string email);
        Task<List<Employee>> GetUsers();
        Task CreateUser(Employee newUser);
        Task UpdatePsychrometricChart(string reportJson, Psychrometric report, string action);
        Task UpdateReport(string reportType, string jobId);
    }
}