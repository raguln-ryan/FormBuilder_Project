using FormBuilder.API.Models.Document;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using FormBuilder.API.Configurations;

namespace FormBuilder.API.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<FormContent> FormContents => _database.GetCollection<FormContent>("form_contents");
        public IMongoCollection<Response> Responses => _database.GetCollection<Response>("responses");
    }
}
