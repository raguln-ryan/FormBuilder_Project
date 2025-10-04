using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.Models.Relational;

namespace FormBuilder.API.DataAccessLayer.Interfaces
{
    public interface IFormMetadataDAL
    {
        Task<List<FormMetadata>> GetAllAsync();
        Task<FormMetadata> GetByIdAsync(int id);
        Task CreateAsync(FormMetadata entity);
        Task<bool> UpdateAsync(int id, FormMetadata entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> PublishAsync(int id);
    }
}
