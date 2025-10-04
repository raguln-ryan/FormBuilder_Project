using System.Threading.Tasks;
using FormBuilder.API.DataAccessLayer.Interfaces;
using FormBuilder.API.Models.Document;
using FormBuilder.API.Data;
using MongoDB.Driver;

namespace FormBuilder.API.DataAccessLayer.Implementations
{
    public class FormContentDAL : IFormContentDAL
    {
        private readonly MongoDbContext _mongoContext;

        public FormContentDAL(MongoDbContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

       

        // Original async methods
        public async Task<bool> AddSectionAsync(int formId, Section section)
        {
            var filter = Builders<FormContent>.Filter.Eq(f => f.FormId, formId);
            var update = Builders<FormContent>.Update.Push(f => f.Sections, section);
            var result = await _mongoContext.FormContents.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<FormContent> GetByFormIdAsync(int formId)
        {
            return await _mongoContext.FormContents.Find(f => f.FormId == formId).FirstOrDefaultAsync();
        }
    }
}
