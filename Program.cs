using cr_app_webapi.Middleware;
using cr_app_webapi.Models;
using cr_app_webapi.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<CodeRedDatabaseSettings>(
    builder.Configuration.GetSection("CodeRedDatabase")
);
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("Auth")
);
builder.Services.AddSingleton<CodeRedServices>();
builder.Services.AddSingleton<AuthServices>();
//builder.Services.AddSingleton<IHttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuth();
//app.UseHttpsRedirection();

app.MapControllers();

app.Run();
