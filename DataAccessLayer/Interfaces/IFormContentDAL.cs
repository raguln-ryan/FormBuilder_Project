using System.Threading.Tasks;
using FormBuilder.API.Models.Document;

namespace FormBuilder.API.DataAccessLayer.Interfaces
{
    public interface IFormContentDAL
    {
        Task<bool> AddSectionAsync(int formId, Section section);
        Task<FormContent> GetByFormIdAsync(int formId);
    }
}
