using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Models;
using System;
using System.Linq;
using System.Security.Claims;
using MongoDB.Bson;
namespace FormBuilder.API.Business.Implementations
{
    public class FormManager : IFormManager
    {
        private readonly IFormRepository _formRepository;

        public FormManager(IFormRepository formRepository)
        {
            _formRepository = formRepository;
        }

        // -------------------- Form Configuration --------------------
        public (bool Success, string Message, FormConfigDto? Data) CreateFormConfig(FormConfigDto dto, string adminUser)
        {
            var form = new Form
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = (FormStatus)dto.Status,
                CreatedBy = adminUser,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _formRepository.Add(form);

            // Return the DTO with the generated FormId
            dto.FormId = form.Id;  // Add the generated ID to the response

            return (true, "Form configuration created successfully", dto);
        }

        public (bool Success, string Message, FormConfigDto? Data) UpdateFormConfig(string id, FormConfigDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            // CHECK: Prevent update if form is published
            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot update configuration of a published form.", null);
            }
            form.Title = dto.Title;
            form.Description = dto.Description;
            form.Status = (FormStatus)dto.Status;
            form.UpdatedAt = DateTime.UtcNow;

            _formRepository.Update(form);
            return (true, "Form configuration updated successfully", dto);
        }

        // -------------------- Form Layout --------------------
        public (bool Success, string Message, FormLayoutDto? Data) CreateFormLayout(FormLayoutDto dto, string adminUser)
        {
            var form = _formRepository.GetById(dto.FormId);
            if (form == null) return (false, "Form not found", null);

            // CHECK: Prevent layout creation if form is already published
            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot modify layout of a published form.", null);
            }
            // Only update questions
            form.Questions = dto.Questions?.Select(q => new Question
            {
                Id = ObjectId.GenerateNewId().ToString(),  // CHANGE THIS LINE - Use ObjectId instead of Guid
                Text = q.Text,
                Type = q.Type,
                Options = q.Options?.ToList() ?? new List<string>(),
                Required = q.Required,
                Description = q.Description,
                MaxLength = q.MaxLength,
                Enabled = q.Enabled,
                CreatedBy = adminUser,  // This is stored in DB but not returned in response
                CreatedAt = DateTime.UtcNow,  // This is stored in DB but not returned in response
                UpdatedAt = DateTime.UtcNow  // This is stored in DB but not returned in response
            }).ToList() ?? new List<Question>();

            form.UpdatedAt = DateTime.UtcNow;
            _formRepository.Update(form);

            // Return DTO without metadata fields
            var responseDto = new FormLayoutDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description,
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options?.ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.Description,
                    MaxLength = q.MaxLength,
                    Enabled = q.Enabled
                    // NO CreatedBy, CreatedAt, UpdatedAt in response!
                }).ToList()
            };

            return (true, "Form layout created successfully", responseDto);
        }
        public (bool Success, string Message, FormLayoutDto? Data) UpdateFormLayout(string id, FormLayoutDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);
            // CHECK: Prevent update if form is published
            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot update layout of a published form.", null);
            }
            if (!string.IsNullOrEmpty(dto.Title))
                form.Title = dto.Title;

            if (!string.IsNullOrEmpty(dto.Description))
                form.Description = dto.Description;

            if (dto.Status.HasValue)
                form.Status = (FormStatus)dto.Status.Value;

            form.Questions = dto.Questions?.Select(q => new Question
            {
                Id = q.Id ?? ObjectId.GenerateNewId().ToString(),  // CHANGE THIS LINE - Use ObjectId
                Text = q.Text,
                Type = q.Type,
                Options = q.Options?.ToList() ?? new List<string>(),
                Required = q.Required,
                Description = q.Description,
                MaxLength = q.MaxLength,
                Enabled = q.Enabled,
                CreatedBy = form.Questions?.FirstOrDefault(eq => eq.Id == q.Id)?.CreatedBy ?? "system",
                CreatedAt = form.Questions?.FirstOrDefault(eq => eq.Id == q.Id)?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList() ?? new List<Question>();

            form.UpdatedAt = DateTime.UtcNow;
            _formRepository.Update(form);

            // Return WITHOUT metadata fields
            var responseDto = new FormLayoutDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description,
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options?.ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.Description,
                    MaxLength = q.MaxLength,
                    Enabled = q.Enabled
                    // NO metadata fields in response!
                }).ToList()
            };

            return (true, "Form layout updated successfully", responseDto);
        }


        // -------------------- Common / Utility --------------------
        public (bool Success, string Message) DeleteForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");

            _formRepository.Delete(id);
            return (true, "Form deleted successfully");
        }

        public object GetAllForms(ClaimsPrincipal user)
        {
            var forms = _formRepository.GetAll();
            return forms.Select(f => new FormLayoutDto
            {
                FormId = f.Id,
                Title = f.Title,
                Description = f.Description,
                Status = (FormStatusDto)f.Status,
                Questions = f.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options?.ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.Description,
                    MaxLength = q.MaxLength,
                    Enabled = q.Enabled
                }).ToList() ?? new List<QuestionDto>()
            }).ToList();
        }

        public (bool Success, string Message, FormLayoutDto? Data) GetFormById(string id, ClaimsPrincipal user)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            var dto = new FormLayoutDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description,
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options?.ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.Description,
                    MaxLength = q.MaxLength,
                    Enabled = q.Enabled
                }).ToList() ?? new List<QuestionDto>()
            };

            return (true, "Form retrieved successfully", dto);
        }

        public (bool Success, string Message) PublishForm(string id, string publishedBy)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");
            // OPTIONAL: Check if form has questions before publishing
            if (form.Questions == null || !form.Questions.Any())
            {
                return (false, "Cannot publish a form without questions");
            }
            form.Status = FormStatus.Published;
            form.UpdatedAt = DateTime.UtcNow;
            form.PublishedBy = publishedBy;
            form.PublishedAt = DateTime.UtcNow;

            _formRepository.Update(form);
            return (true, "Form published successfully");
        }
    }
}

