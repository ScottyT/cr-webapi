using cr_app_webapi.Middleware;
using cr_app_webapi.Models;
using cr_app_webapi.Services;

var MyCorsPolicy = "corsPolicy";
var builder = WebApplication.CreateBuilder(args);
/* var port = Environment.GetEnvironmentVariable("PORT") ?? "8082";
var url = $"http://localhost:{port}";
var appUrl = Environment.GetEnvironmentVariable("APP_URL") ?? "http://localhost:3000"; */

// Add services to the container.
builder.Services.Configure<CodeRedDatabaseSettings>(
    builder.Configuration.GetSection("CodeRedDatabase")
);
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("Auth")
);
builder.Services.AddSingleton<CodeRedServices>();
builder.Services.AddSingleton<AuthServices>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyCorsPolicy,
    builder =>
    {
        builder.WithOrigins("http://localhost:3000")
            .AllowCredentials()
            .WithHeaders("Origin", "X-Requested-With", "Content-Type", "Accept", "Authorization")
            .WithMethods("OPTIONS", "GET", "POST", "PUT", "DELETE");
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null
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
app.UseRouting();
app.UseCors(MyCorsPolicy);
//app.UseAuth();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();
