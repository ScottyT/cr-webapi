using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace cr_app_webapi.Services
{
    public abstract class ErrorLogic : ControllerBase
    {
        public abstract ActionResult ErrorHandling(HttpStatusCode statusCode, string? httpContent);
    }
}