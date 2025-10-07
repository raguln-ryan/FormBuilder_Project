using FormBuilder.API.Configurations;
using MongoDB.Driver;
using FormBuilder.API.Models;


namespace FormBuilder.API.Services
{
    public class MongoDbService
    {
        private readonly MongoDbContext _context;

        public MongoDbService(MongoDbContext context)
        {
            _context = context;
        }

        public IMongoCollection<Form> Forms => _context.Forms;
        public IMongoCollection<Question> Questions => _context.Questions;

        // Add your methods for CRUD operations here
    }
}
