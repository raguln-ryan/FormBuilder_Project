using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IResponseRepository
    {
        void Add(Response response);
        Response? GetById(int id);
        IEnumerable<Response> GetByFormId(int formId);
        IEnumerable<Response> GetAll();   // <-- new method
        void Update(Response response);
        void Delete(int id);
    }
}
