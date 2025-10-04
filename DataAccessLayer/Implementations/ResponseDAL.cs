using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.DataAccessLayer.Interfaces;
using FormBuilder.API.Models.Document;
using FormBuilder.API.Data;
using MongoDB.Driver;

namespace FormBuilder.API.DataAccessLayer.Implementations
{
    public class ResponseDAL : IResponseDAL
    {
        private readonly MongoDbContext _mongoContext;

        public ResponseDAL(MongoDbContext mongoContext)
        {
            _mongoContext = mongoContext;
        }
        // Must be public to implement interface
        public async Task<bool> SubmitResponseAsync(Response response)
        {
            await _mongoContext.Responses.InsertOneAsync(response);
            return true;
        }

        public async Task<List<Response>> GetResponsesByFormAsync(int formId)
        {
            return await _mongoContext.Responses.Find(r => r.FormId == formId).ToListAsync();
        }

        public async Task<Response> GetResponseByIdAsync(int formId, string responseId)
        {
            return await _mongoContext.Responses.Find(r => r.FormId == formId && r.ResponseId == responseId).FirstOrDefaultAsync();
        }
    }
}
