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
    [Authorize("create:user")]
    public class EmployeesController : ControllerBase
    {
        private AuthServices _authService;
        private readonly IMongoRepo<Employee> _userRepo;

        public EmployeesController(AuthServices authService, IMongoRepo<Employee> userRepo)
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
                    fullName = new FullName(projection.fname, projection.lname)
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
                    fullName = new FullName(projection.fname, projection.lname).Name,
                    team_id = projection.team_id,
                    role = projection.role,
                    picture = projection.picture,
                    // Add certifications in the future
                }
            ).FirstOrDefault();
            if (user is null)
            {
                return NoContent();
            }
            
            return user;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(UserObj? e)
        {
            /* Object userInAuth0 = new 
            {
                connection = "Username-Password-Authentication",
                email = newEmployee.email,
                password = newEmployee
            }; */
            /* UserObj authUser = new UserObj
            {
                connection = "Username-Password-Authentication",
                email = e.email,
                password = e.password,
                username = e.username,
                user_metadata = e.user_metadata
            }; */
            //await _userRepo.SaveOneAsync(e.employee);
            await _authService.CreateUser(e);
            return CreatedAtAction(nameof(Get), new { }, "Successfully created new user!");
        }
    }
}