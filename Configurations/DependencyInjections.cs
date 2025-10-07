using FormBuilder.API.Business.Implementations;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DataAccess.Implementations;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder.API.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // MySQL DbContext
            services.AddDbContext<MySqlDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("MySqlConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("MySqlConnection"))
                )
            );

            // MongoDB
            var mongoSettings = configuration.GetSection("MongoSettings");
            var mongoConnectionString = mongoSettings.GetValue<string>("ConnectionString");
            var mongoDatabaseName = mongoSettings.GetValue<string>("DatabaseName");

            services.AddSingleton(new MongoDbContext(mongoConnectionString, mongoDatabaseName));
            services.AddScoped<MongoDbService>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();

            // Managers
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddScoped<IFormManager, FormManager>();
            services.AddScoped<IResponseManager, ResponseManager>();

            // JWT Service
            services.AddScoped<JwtService>(x =>
                new JwtService(configuration["JwtSecret"], int.Parse(configuration["JwtExpiryMinutes"])));

            services.AddScoped<PasswordHasher>();
            services.AddScoped<MySqlService>();

            return services;
        }
    }
}
