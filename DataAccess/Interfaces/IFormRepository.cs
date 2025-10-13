using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IFormRepository
    {
        void Add(Form form);
        Form GetById(string id);
        IEnumerable<Form> GetAll();
        IEnumerable<Form> GetByStatus(FormStatus status);
        void Update(Form form);
        void Delete(string id);
        
        // New methods
        Form CreateConfig(string title, string description, string createdBy);
        Form CreateLayout(string formId, List<Question> questions, string updatedBy);
        
        // Deletes a form and all its responses
        void DeleteFormAndResponses(string formId, IResponseRepository responseRepository);

        // -----------------------------
        // Partial update methods
        // -----------------------------
        void UpdateConfig(string formId, string title, string description);
        void UpdateLayout(string formId, List<Question> questions);
        void PublishForm(string formId, string publishedBy, System.DateTime publishedAt);
    }
}
