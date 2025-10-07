using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IQuestionRepository
    {
        void Add(Question question);
        Question GetById(string id);
        IEnumerable<Question> GetAll();
        void Update(Question question);
        void Delete(string id);
    }
}
