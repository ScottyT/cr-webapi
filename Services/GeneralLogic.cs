using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace cr_app_webapi.Services
{
    public abstract class GeneralLogic : ControllerBase
    {
        public abstract ActionResult ErrorHandling(HttpStatusCode statusCode, string? httpContent);
    }
}