using cr_app_webapi.Models;
using Microsoft.Extensions.Options;

namespace cr_app_webapi.Services
{
    public class EmployeeService : ILogic<IEmployee>
    {
        protected readonly MyDatabase<IEmployee> Employees;
        protected readonly GlobalLogic<IEmployee, Employee> globalLogic = new GlobalLogic<IEmployee, Employee>();
        
        public EmployeeService(IOptions<CodeRedDatabaseSettings> settings)
        {
            Employees = new MyDatabase<IEmployee>("employees", settings.Value.ConnectionString, settings.Value.DatabaseName);
        }
        public Task<IEnumerable<IEmployee>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEmployee> GetManyAsync(IEnumerable<IEmployee> contexts)
        {
            throw new NotImplementedException();
        }

        public Task<IEmployee> GetManyAsync(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEmployee> GetOneAsync(IEmployee context)
        {
            throw new NotImplementedException();
        }

        public Task<IEmployee> GetOneAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveManyAsync(IEnumerable<IEmployee> contexts)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveManyAsync(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveOneAsync(IEmployee context)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveOneAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEmployee> SaveManyAsync(IEnumerable<IEmployee> contexts)
        {
            throw new NotImplementedException();
        }

        public Task<IEmployee> SaveOneAsync(IEmployee Context)
        {
            throw new NotImplementedException();
        }
    }
}