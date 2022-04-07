using System.Net;
using System.Text.Json;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace cr_app_webapi
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize("read:users")]
    [Authorize("create:user")]
    /* [Authorize("update:roles")] */
    public class EmployeesController : GeneralLogic
    {
        private AuthServices _authService;
        private readonly IMongoRepo<Employee,Employee> _userRepo;

        public EmployeesController(AuthServices authService, IMongoRepo<Employee,Employee> userRepo)
        {
            _userRepo = userRepo;
            _authService = authService;
        }

        [HttpGet]
        public IEnumerable<Object> Get()
        {
            var user = _userRepo.FilterBy(
                _ => true, projection => new
                {
                    email = projection.email,
                    role = projection.role,
                    team_id = projection.team_id,
                    fullName = projection.name
                }
            );
            return user.ToList();
        }
            

        [HttpGet("{email}")]
        public ActionResult<Object> GetUser(string email, string id)
        {
            var user = _userRepo.FilterBy(
                filter => filter.email == email,
                projection => new
                {
                    email = projection.email,
                    fullName = projection.name,
                    team_id = projection.team_id,
                    role = projection.role,
                    picture = projection.picture,
                    auth_id = projection.auth_id
                    // Add certifications in the future
                }
            ).FirstOrDefault();
            if (user is null)
            {
                return NoContent();
            }
            
            return user;
        }

        [HttpPut("update/{id}")]
        public async Task UpdateUser(string id, Employee emp)
        {
            await _authService.UpdateUser(emp, id);
            await _userRepo.GenericFindOneUpdate<Employee>(
                f => f.auth_id == id, emp, true
            );
        }

        [HttpPost("auth/create")]
        public async Task<IActionResult> CreateAuth0User(UserObj user)
        {
            AuthUser userInAuth0 = new AuthUser
            {
                connection = "Username-Password-Authentication",
                email = user.email,
                password = user.password,
                username = user.username,
                name = new FullName(user.fname, user.lname).Name,
                user_metadata = new UserMetadata { 
                    certifications = new List<string>(),
                    role = user.role,
                    id = user.team_id
                }
            };
            Employee employee = new Employee
            {
                email = user.email,
                fname = user.fname,
                lname = user.lname,
                name = new FullName(user.fname, user.lname).Name,
                username = user.username,
                team_id = user.team_id,
                role = user.role
            };
            if (user is null) return BadRequest();

            RestResponse response = await _authService.CreateUser(userInAuth0);
            if (!response.IsSuccessful)
            {
                return ErrorHandling(response.StatusCode, response.Content);
            }

            if (employee is null) return BadRequest("Can't add userto database");
            
            return Ok("Successfully created new user!");
        }
        
        /* [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] UserObj newUser)
        {
            Employee employee = new Employee
            {
                email = newUser.email,
                fname = newUser.fname,
                lname = newUser.lname,
                name = new FullName(newUser.fname, newUser.lname).Name,
                username = newUser.username,
                team_id = newUser.team_id,
                role = newUser.role
            };

            if (newUser is null) return BadRequest();

            await _userRepo.InsertOneAsync(employee);
            return CreatedAtAction(nameof(Get), "Successfully created new user!");
        } */

        public override ActionResult ErrorHandling(HttpStatusCode statusCode, string? httpContent)
        {
            if (!ValidExtensions.IsValid<string>(httpContent))
            {
                return BadRequest();
            }
            var errorContent = JsonSerializer.Deserialize<object>(httpContent);
            if (statusCode == HttpStatusCode.Conflict)
            {
                return Conflict(errorContent);
            }
            if (statusCode == HttpStatusCode.NotFound)
            {
                return NotFound(errorContent);
            }
            return BadRequest();
        }
    }
}