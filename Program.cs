using System.Net;
using System.Security.Claims;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var MyCorsPolicy = "corsPolicy";
var builder = WebApplication.CreateBuilder(args);
/* var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}"; */
builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    options.Listen(IPAddress.Any, int.Parse(port));
});
//builder.WebHost.UseUrls("http://0.0.0.0:8080");
{
    var services = builder.Services;
    // Commenting this out because using this with Google API Gateway cause this app to always have unauthorized access. If use with Azure API Management this wont happen
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.Authority = builder.Configuration.GetSection("Auth0")["Authority"]; //builder.Configuration["Auth0:Authority"];
        options.Audience = builder.Configuration.GetSection("Auth0")["Audience"]; //builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });
    services.AddAuthorization(options =>
    {
        var domain = builder.Configuration.GetSection("Auth0")["Authority"];
        options.AddPolicy("read:users", policy => policy.Requirements.Add(new HasScopeRequirement("read:users", domain)));
        options.AddPolicy("read:reports", policy => policy.Requirements.Add(new HasScopeRequirement("read:reports", domain)));
        options.AddPolicy("create:user", policy => policy.Requirements.Add(new HasScopeRequirement("create:user", domain)));
        options.AddPolicy("update:roles", policy => policy.Requirements.Add(new HasScopeRequirement("update:roles", domain)));
    });
    // Add services to the container.
    services.Configure<CodeRedDatabaseSettings>(
        builder.Configuration.GetSection("CodeRedDatabase")
    );
    services.Configure<Auth0Settings>(
        builder.Configuration.GetSection("Auth0")
    );
    services.AddScoped(typeof(IMongoRepo<,>), typeof(MongoRepo<,>));
    services.AddTransient<AuthServices>();
    services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    services.AddSingleton<ICodeRedDatabaseSettings>(sp =>
        sp.GetRequiredService<IOptions<CodeRedDatabaseSettings>>().Value);
    services.AddCors(options =>
    {
        options.AddPolicy(name: MyCorsPolicy,
        b =>
        {
            b.WithOrigins("http://localhost:3000", "https://staging-174a0.web.app", "https://code-red-app-313517.web.app")
                .AllowCredentials()
                .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization")
                .WithMethods("OPTIONS", "GET", "POST", "PUT", "DELETE");
        });
    });

    services.AddControllers().AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        }
    );
    services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });
    services.AddHttpContextAccessor();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
        config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "CodeRedAppApi",
            Version = "v1",
        });
    });
}

var app = builder.Build();
app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });
    //app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else 
{
    app.UseHsts();
}

app.UseSwaggerUI(c =>
{
    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeRedApp");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeRedAppApi");
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
app.Run();
