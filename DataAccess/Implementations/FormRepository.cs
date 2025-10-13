using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using MongoDB.Driver;
using System;
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

        // -----------------------------
        // Create Config - Creates a new form with basic configuration
        // -----------------------------
        public Form CreateConfig(string title, string description, string createdBy)
        {
            var form = new Form
            {
                Title = title,
                Description = description,
                Status = FormStatus.Draft,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Questions = new List<Question>() // Initialize with empty questions list
            };

            _forms.InsertOne(form);
            return form;
        }

        // -----------------------------
        // Create Layout - Creates or updates form with questions
        // -----------------------------
        public Form CreateLayout(string formId, List<Question> questions, string updatedBy)
        {
            // First check if form exists
            var existingForm = GetById(formId);
            
            if (existingForm == null)
            {
                // Create new form with layout
                var form = new Form
                {
                    Id = formId,
                    Title = "Untitled Form", // Default title
                    Description = "",
                    Status = FormStatus.Draft,
                    CreatedBy = updatedBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Questions = questions
                };
                
                _forms.InsertOne(form);
                return form;
            }
            else
            {
                // Update existing form with new questions
                var update = Builders<Form>.Update
                    .Set(f => f.Questions, questions)
                    .Set(f => f.UpdatedAt, DateTime.UtcNow);
                
                _forms.UpdateOne(f => f.Id == formId, update);
                
                // Return updated form
                return GetById(formId);
            }
        }

        // -----------------------------
        // Optional: Delete form + all responses
        // -----------------------------
        public void DeleteFormAndResponses(string formId, IResponseRepository responseRepository)
        {
            var responses = responseRepository.GetByFormId(formId);
            foreach (var response in responses)
            {
                responseRepository.Delete(response.Id.ToString());
            }

            _forms.DeleteOne(f => f.Id == formId);
        }

        // -----------------------------
        // Partial Updates
        // -----------------------------
        public void UpdateConfig(string formId, string title, string description)
        {
            var update = Builders<Form>.Update
                .Set(f => f.Title, title)
                .Set(f => f.Description, description)
                .Set(f => f.UpdatedAt, DateTime.UtcNow);
            _forms.UpdateOne(f => f.Id == formId, update);
        }

        public void UpdateLayout(string formId, List<Question> questions)
        {
            var update = Builders<Form>.Update
                .Set(f => f.Questions, questions)
                .Set(f => f.UpdatedAt, DateTime.UtcNow);
            _forms.UpdateOne(f => f.Id == formId, update);
        }

        public void PublishForm(string formId, string publishedBy, DateTime publishedAt)
        {
            var update = Builders<Form>.Update
                .Set(f => f.Status, FormStatus.Published)
                .Set(f => f.PublishedBy, publishedBy)
                .Set(f => f.PublishedAt, publishedAt)
                .Set(f => f.UpdatedAt, DateTime.UtcNow);
            _forms.UpdateOne(f => f.Id == formId, update);
        }
    }
}
