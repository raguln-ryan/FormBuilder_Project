using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using MongoDB.Driver;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Implementations
{
    public class FormRepository : IFormRepository
    {
        private readonly IMongoCollection<Form> _forms;

        public FormRepository(MongoDbContext dbContext)
        {
            _forms = dbContext.Forms;
        }

        public void Add(Form form) => _forms.InsertOne(form);

        public Form GetById(string id) => _forms.Find(f => f.Id == id).FirstOrDefault();

        public IEnumerable<Form> GetAll() => _forms.Find(f => true).ToList();

        public IEnumerable<Form> GetByStatus(FormStatus status) =>
            _forms.Find(f => f.Status == status).ToList();

        public void Update(Form form) =>
            _forms.ReplaceOne(f => f.Id == form.Id, form);

        public void Delete(string id) =>
            _forms.DeleteOne(f => f.Id == id);

        public void DeleteFormAndResponses(string formId, IResponseRepository responseRepository)
        {
            // 1️⃣ Delete all responses for this form in MySQL
            var responses = responseRepository.GetByFormId(formId);
            foreach (var response in responses)
            {
                responseRepository.Delete(response.Id.ToString());
            }

            // 2️⃣ Delete the form in MongoDB
            _forms.DeleteOne(f => f.Id == formId);
        }

    }
}
