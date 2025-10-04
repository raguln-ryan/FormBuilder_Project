using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.DTOs;

namespace FormBuilder.API.BusinessLayer.Interfaces
{
    public interface IFormBL
    {
        Task<List<FormMetadataDTO>> GetAllFormsAsync();
        Task<FormMetadataDTO> GetFormByIdAsync(int id);
        Task<FormMetadataDTO> CreateFormAsync(FormMetadataDTO formDto);
        Task<bool> UpdateFormAsync(int id, FormMetadataDTO formDto);
        Task<bool> DeleteFormAsync(int id);
        Task<bool> PublishFormAsync(int id);
        Task<bool> AddSectionAsync(int formId, SectionDTO sectionDto);
    }
}
