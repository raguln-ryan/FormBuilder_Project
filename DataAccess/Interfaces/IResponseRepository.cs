using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IResponseRepository
    {
        void Add(Response response);
        Response? GetById(string id); // ✅ int
        IEnumerable<Response> GetByFormId(string formId);
        IEnumerable<Response> GetAll();
        void Update(Response response);
        void Delete(string id); // ✅ int
    }
}
