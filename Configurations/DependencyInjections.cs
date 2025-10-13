using FormBuilder.API.Business.Implementations;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DataAccess.Implementations;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FormBuilder.API.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MySqlDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("MySqlConnection") ?? throw new Exception("MySQL connection string missing"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("MySqlConnection"))
                )
            );

            var mongoSettings = configuration.GetSection("MongoSettings");
            var mongoConnectionString = mongoSettings.GetValue<string>("ConnectionString") ?? throw new Exception("MongoDB connection string missing");
            var mongoDatabaseName = mongoSettings.GetValue<string>("DatabaseName") ?? throw new Exception("MongoDB database name missing");

            services.AddSingleton(new MongoDbContext(mongoConnectionString, mongoDatabaseName));
            services.AddScoped<MongoDbService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();

            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IFormManager, FormManager>();
            services.AddScoped<IResponseManager, ResponseManager>();

            var jwtSecret = configuration["JwtSecret"] ?? throw new Exception("JWT secret missing");
            var jwtExpiry = int.TryParse(configuration["JwtExpiryMinutes"], out int expiryMinutes) ? expiryMinutes : 60;
            services.AddScoped<JwtService>(x => new JwtService(jwtSecret, jwtExpiry));

            services.AddScoped<PasswordHasher>();
            services.AddScoped<MySqlService>();

            return services;
        }
    }
}
