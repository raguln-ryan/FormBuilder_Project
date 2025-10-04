using FormBuilder.API.Data;
using FormBuilder.API.BusinessLayer.Interfaces;
using FormBuilder.API.BusinessLayer.Implementations;
using FormBuilder.API.DataAccessLayer.Interfaces;
using FormBuilder.API.DataAccessLayer.Implementations;
using FormBuilder.API.Mappers;
using FormBuilder.API.Middleware;
using Microsoft.EntityFrameworkCore;
using FormBuilder.API.Configurations;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// MySQL DbContext
builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
    new MySqlServerVersion(new Version(8, 0, 33))));

// MongoDB
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
builder.Services.AddSingleton<MongoDbContext>();

// BusinessLayer
builder.Services.AddScoped<IFormBL, FormBL>();
builder.Services.AddScoped<IResponseBL, ResponseBL>();

// DataAccessLayer
builder.Services.AddScoped<IFormMetadataDAL, FormMetadataDAL>();
builder.Services.AddScoped<IFormContentDAL, FormContentDAL>();
builder.Services.AddScoped<IResponseDAL, ResponseDAL>();

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger – always enabled
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FormBuilder API V1");
    c.RoutePrefix = string.Empty; // Swagger at root: https://localhost:5001/
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<MySqlDbContext>();
if (db.Database.CanConnect())
{
    Console.WriteLine("✅ Connected to MySQL!");
}
else
{
    Console.WriteLine("❌ Cannot connect to MySQL!");
}

app.Run();
