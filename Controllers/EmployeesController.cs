using System.Text.Json;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cr_app_webapi
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize("read:users")]
    public class EmployeesController : ControllerBase
    {
        private AuthServices _authService;
        private readonly IMongoDbClient _mongoService;
        public EmployeesController(AuthServices authService, IMongoDbClient mongoService)
        {
            _authService = authService;
            _mongoService = mongoService;
        }

        [HttpGet]
        public async Task<List<Employee>> Get()
        {
            var user = await _mongoService.GetUsers();
            return user;
        }
            

        [HttpGet("{email}")]
        public async Task<ActionResult<Object?>> GetUser(string email, string id)
        {
            var authuser = await _authService.GetUser(id); //This gets the user data from Auth0
            var userdata = await _mongoService.GetUser(email); //This gets the user data from the MongoDb database
            if (authuser.Content is null)
            {
                return NotFound();
            }
            var content = JsonSerializer.Deserialize<Object>(authuser.Content);
            return content;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(Employee newEmployee)
        {
            await _mongoService.CreateUser(newEmployee);
            return CreatedAtAction(nameof(Get), new { _id = newEmployee._id }, newEmployee);
        }
    }
}