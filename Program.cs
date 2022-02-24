using System.Security.Claims;
using System.Text.Json.Serialization;
using cr_app_webapi;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using JwtAuthentication.AsymmetricEncryption.Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var MyCorsPolicy = "corsPolicy";
var builder = WebApplication.CreateBuilder(args);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8082";
var url = $"http://localhost:{port}";
var appUrl = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:3000";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => 
{
    options.SaveToken = true;
    options.Authority = builder.Configuration["Auth0:Authority"];
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
    
});
builder.Services.AddAuthorization(options =>
{
    var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.AddPolicy("read:users", policy => policy.Requirements.Add(new HasScopeRequirement("read:users", domain)));
    options.AddPolicy("read:reports", policy => policy.Requirements.Add(new HasScopeRequirement("read:reports", domain)));
});
// Add services to the container.
builder.Services.Configure<CodeRedDatabaseSettings>(
    builder.Configuration.GetSection("CodeRedDatabase")
);
builder.Services.Configure<Auth0Settings>(
    builder.Configuration.GetSection("Auth0")
);
builder.Services.AddSingleton<CodeRedServices>();
builder.Services.AddSingleton<AuthServices>();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
builder.Services.AddSingleton<IMongoDbClient>(
    new MongoDbClient(
        builder.Configuration["MongoDb:ConnectionString"], builder.Configuration["MongoDb:Database"]
    )
);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy,
    builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://staging-174a0.web.app")
            .AllowCredentials()
            .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization")
            .WithMethods("OPTIONS", "GET", "POST", "PUT", "DELETE");
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        }
    );
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CodeRedApp",
        Version = "v1",
    });
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    /* app.UseSwagger();
    app.UseSwaggerUI(); */
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeRedApp");
});
app.UseHttpsRedirection();
app.UseCors(MyCorsPolicy);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run(url);
