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
    }
}
