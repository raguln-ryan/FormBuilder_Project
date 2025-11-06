using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using MongoDB.Driver;
using MongoDB.Bson;
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

        public void Delete(string id)
        {
            var result = _forms.DeleteOne(f => f.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException($"Form with ID {id} could not be deleted or does not exist.");
            }
        }

        // -----------------------------
        // NEW METHOD: Get Paginated Forms with Filtering at MongoDB level
        // -----------------------------
        public (IEnumerable<Form> Forms, int TotalCount) GetPaginatedForms(int skip, int take, string searchTerm = null)
        {
            var filterBuilder = Builders<Form>.Filter;
            var filter = filterBuilder.Empty;

            // Build search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchRegex = new BsonRegularExpression(searchTerm, "i"); // case-insensitive
                
                // Create filters for each field
                var titleFilter = filterBuilder.Regex(f => f.Title, searchRegex);
                // var descriptionFilter = filterBuilder.Regex(f => f.Description, searchRegex);
                
                // For nested questions search
                // var questionTextFilter = filterBuilder.ElemMatch(f => f.Questions,
                //     Builders<Question>.Filter.Regex(q => q.QuestionText, searchRegex));
                
                // var questionDescFilter = filterBuilder.ElemMatch(f => f.Questions,
                //     Builders<Question>.Filter.Regex(q => q.Description, searchRegex));

                // Combine all filters with OR
                filter = filterBuilder.Or(
                    titleFilter
                );
            }

            // Get total count with filter applied
            var totalCount = (int)_forms.CountDocuments(filter);

            // Get paginated results with filter, sorted by UpdatedAt descending
            var forms = _forms
                .Find(filter)
                .SortByDescending(f => f.UpdatedAt)
                .Skip(skip)
                .Limit(take)
                .ToList();

            return (forms, totalCount);
        }

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
        // Delete form + all responses (Matches interface with void return)
        // -----------------------------
        public void DeleteFormAndResponses(string formId, IResponseRepository responseRepository)
        {
            // Check if form exists
            var form = GetById(formId);
            if (form == null)
            {
                throw new InvalidOperationException($"Form with ID {formId} not found");
            }

            // Get and delete all responses
            var responses = responseRepository.GetByFormId(formId);
            var responseCount = 0;
            var deletedCount = 0;
            var failedDeletions = new List<string>();
            
            foreach (var response in responses)
            {
                responseCount++;
                try
                {
                    responseRepository.Delete(response.Id.ToString());
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    failedDeletions.Add($"ResponseId: {response.Id} - {ex.Message}");
                }
            }

            // Only delete form if all responses were deleted successfully
            if (responseCount > 0 && deletedCount != responseCount)
            {
                throw new InvalidOperationException(
                    $"Only {deletedCount} of {responseCount} responses could be deleted. " +
                    $"Failed deletions: {string.Join("; ", failedDeletions)}. Form not deleted.");
            }

            // Delete the form
            var result = _forms.DeleteOne(f => f.Id == formId);
            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException($"Failed to delete form with ID {formId}");
            }
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
