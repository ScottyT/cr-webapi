using Microsoft.AspNetCore.Mvc;

namespace cr_app_webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ValuesController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            return _config["Auth0:Domain"];
        }
    }
}