using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IResponseRepository
    {
        void Add(Response response);
        Response? GetById(string id);
        IEnumerable<Response> GetByFormId(string formId);
        IEnumerable<Response> GetAll();
        void Update(Response response);
        void Delete(string id);
        
        // NEW METHOD - Get responses by user ID
        IEnumerable<Response> GetByUserId(int userId);
        
        // ADD THIS NEW METHOD
        int DeleteAllByFormId(string formId);
    }
}
