using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.BusinessLayer.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.DataAccessLayer.Interfaces;
using AutoMapper;

namespace FormBuilder.API.BusinessLayer.Implementations
{
    public class FormBL : IFormBL
    {
        private readonly IFormMetadataDAL _formMetadataDAL;
        private readonly IFormContentDAL _formContentDAL;
        private readonly IMapper _mapper;

        public FormBL(IFormMetadataDAL formMetadataDAL, IFormContentDAL formContentDAL, IMapper mapper)
        {
            _formMetadataDAL = formMetadataDAL;
            _formContentDAL = formContentDAL;
            _mapper = mapper;
        }

        public async Task<List<FormMetadataDTO>> GetAllFormsAsync()
        {
            var entities = await _formMetadataDAL.GetAllAsync();
            return _mapper.Map<List<FormMetadataDTO>>(entities);
        }

        public async Task<FormMetadataDTO> GetFormByIdAsync(int id)
        {
            var entity = await _formMetadataDAL.GetByIdAsync(id);
            return _mapper.Map<FormMetadataDTO>(entity);
        }

        public async Task<FormMetadataDTO> CreateFormAsync(FormMetadataDTO formDto)
        {
            var entity = _mapper.Map<Models.Relational.FormMetadata>(formDto);
            await _formMetadataDAL.CreateAsync(entity);
            return _mapper.Map<FormMetadataDTO>(entity);
        }

        public async Task<bool> UpdateFormAsync(int id, FormMetadataDTO formDto)
        {
            var entity = _mapper.Map<Models.Relational.FormMetadata>(formDto);
            return await _formMetadataDAL.UpdateAsync(id, entity);
        }

        public async Task<bool> DeleteFormAsync(int id)
        {
            return await _formMetadataDAL.DeleteAsync(id);
        }

        public async Task<bool> PublishFormAsync(int id)
        {
            return await _formMetadataDAL.PublishAsync(id);
        }

        public async Task<bool> AddSectionAsync(int formId, SectionDTO sectionDto)
        {
            var section = _mapper.Map<Models.Document.Section>(sectionDto);
            return await _formContentDAL.AddSectionAsync(formId, section);
        }
    }
}
