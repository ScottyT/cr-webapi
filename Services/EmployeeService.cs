using cr_app_webapi.Models;
using Microsoft.Extensions.Options;

namespace cr_app_webapi.Services
{
    /* public class EmployeeService : IMongoRepo<EmployeeContext>
    {
        protected readonly MyDatabase<IEmployee> Employees;
        protected readonly GlobalLogic<IEmployee, Employee> globalLogic = new GlobalLogic<IEmployee, Employee>();
        
        public EmployeeService(ICodeRedDatabaseSettings settings)
        {
            Employees = new MyDatabase<IEmployee>("employees", settings.ConnectionString, settings.DatabaseName);
        }
        public async Task<IEnumerable<EmployeeContext>> GetAllAsync()
        {
            var employees = await globalLogic.GetAllAsync(Employees.Collection);
            return employees.Select(e => new EmployeeContext
            {
                Id = e.Id.ToString(),
                email = e.email,
                role = e.role
            });
        }

        public Task<EmployeeContext> GetManyAsync(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        
        public async Task<EmployeeContext> GetOneAsync(string id)
        {
            var employee = await globalLogic.GetOneAsync(Employees.Collection, id);
            return employee.ToEmployeeContext();
        }

       

        public Task<bool> RemoveManyAsync(IEnumerable<Employee> contexts)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveManyAsync(IEnumerable<EmployeeContext> contexts)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveOneAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<EmployeeContext> SaveOneAsync(EmployeeContext context)
        {
            var employee = context.AsNewEmployee();
            await Employees.Collection.InsertOneAsync(employee);
            context.Id = employee.Id.ToString();
            return context;
        }
    }

    public static class EmployeeExtensions
    {
        public static EmployeeContext ToEmployeeContext(this IEmployee employee)
        {
            return new EmployeeContext
            {
                email = employee.email,
                fullName = new FullName(employee.fname, employee.lname),
                team_id = employee.team_id,
                role = employee.role
            };
        }
        public static Employee AsNewEmployee(this EmployeeContext context)
        {
            return new Employee
            {
                email = context.email,
                fname = context.fname,
                lname = context.lname,
                role = context.role,
                team_id = context.team_id
            };
        }
    } */
}