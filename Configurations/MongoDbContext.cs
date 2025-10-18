using FormBuilder.API.Models;
using MongoDB.Driver;

namespace FormBuilder.API.Configurations
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public virtual IMongoCollection<Form> Forms => _database.GetCollection<Form>("Forms");
        public virtual IMongoCollection<Question> Questions => _database.GetCollection<Question>("Questions");
    }
}
