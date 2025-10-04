using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.Models.Document;

namespace FormBuilder.API.DataAccessLayer.Interfaces
{
    public interface IResponseDAL
    {
        Task<bool> SubmitResponseAsync(Response response);
        Task<List<Response>> GetResponsesByFormAsync(int formId);
        Task<Response> GetResponseByIdAsync(int formId, string responseId);
    }
}
