using System.Security.Claims;
using System.Text.Json.Serialization;
using cr_app_webapi;
using cr_app_webapi.Helpers;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var MyCorsPolicy = "corsPolicy";
var builder = WebApplication.CreateBuilder(args);
/* builder.Services.AddSpaStaticFiles(options =>
{
    options.RootPath = "client_app/dist";
}); */
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
    options.AddPolicy("create:user", policy => policy.Requirements.Add(new HasScopeRequirement("create:user", domain)));
});
// Add services to the container.
builder.Services.Configure<CodeRedDatabaseSettings>(
    builder.Configuration.GetSection("CodeRedDatabase")
);
builder.Services.Configure<Auth0Settings>(
    builder.Configuration.GetSection("Auth0")
);
builder.Services.AddScoped(typeof(IMongoRepo<,>), typeof(MongoRepo<,>));
builder.Services.AddSingleton<AuthServices>();
builder.Services.AddTransient<ReportsService>();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddSingleton<ICodeRedDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<CodeRedDatabaseSettings>>().Value);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy,
    b =>
    {
        b.WithOrigins("https://localhost:7255", "https://localhost:8000", "https://staging-174a0.web.app", "https://code-red-app-313517.web.app")
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

var app = builder.Build();
IWebHostEnvironment env = app.Environment;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    /* app.UseSwagger();
    app.UseSwaggerUI(); */
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseCors(MyCorsPolicy);
app.UseStaticFiles();
/* app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "client_app";
    if (env.IsDevelopment()) {
        // Launch the server for Nuxt
        spa.UseNuxtDevelopmentServer();
    }
}); */
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"
);
app.MapFallbackToFile("index.html");
app.Run();
