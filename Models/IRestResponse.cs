namespace cr_app_webapi.Models
{
    internal interface IRestResponse
    {
        string? access_token {get; set;}
        string? token_type {get; set;}
    }
}