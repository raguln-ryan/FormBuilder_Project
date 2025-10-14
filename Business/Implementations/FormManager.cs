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
        private readonly IResponseRepository _responseRepository;

        public FormManager(IFormRepository formRepository, IResponseRepository responseRepository)
        {
            _formRepository = formRepository;
            _responseRepository = responseRepository;
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

            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot modify layout of a published form.", null);
            }
            
            // Use the values from input instead of calculating them
            form.Questions = dto.Questions?.Select((q, index) => new Question
            {
                QuestionId = ObjectId.GenerateNewId().ToString(),
                QuestionText = q.Text,
                Type = q.Type,
                DescriptionEnabled = q.DescriptionEnabled,  // ✅ Use input value
                Description = q.Description ?? "",
                SingleChoice = q.SingleChoice,              // ✅ Use input value
                MultipleChoice = q.MultipleChoice,          // ✅ Use input value
                Options = q.Options?.Select(opt => new Option
                {
                    OptionId = Guid.NewGuid().ToString(),
                    Value = opt
                }).ToList(),
                Format = q.Format,                           // ✅ Use input value
                Required = q.Required,
                Order = q.Order                              // ✅ Use input value
            }).ToList() ?? new List<Question>();

            form.UpdatedAt = DateTime.UtcNow;
            form.UpdatedBy = adminUser;
            
            _formRepository.Update(form);

            // Return response
            var responseDto = new FormLayoutDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description,
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions.Select(q => new QuestionDto
                {
                    Id = q.QuestionId,
                    Text = q.QuestionText,
                    Type = q.Type,
                    Options = q.Options?.Select(o => o.Value).ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.Description,
                    DescriptionEnabled = q.DescriptionEnabled,    // ✅ Return actual value
                    SingleChoice = q.SingleChoice,                // ✅ Return actual value
                    MultipleChoice = q.MultipleChoice,            // ✅ Return actual value
                    Format = q.Format,                            // ✅ Return actual value
                    Order = q.Order                               // ✅ Return actual value
                }).ToList()
            };

            return (true, "Form layout created successfully", responseDto);
        }
        public (bool Success, string Message, FormLayoutDto? Data) UpdateFormLayout(string id, FormLayoutDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);
            
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

            // Update questions with new model structure
            form.Questions = dto.Questions?.Select((q, index) => new Question
            {
                QuestionId = q.Id ?? ObjectId.GenerateNewId().ToString(),
                QuestionText = q.Text,
                Type = q.Type,
                DescriptionEnabled = !string.IsNullOrEmpty(q.Description),
                Description = q.Description ?? "",
                SingleChoice = q.Type == "radio" || q.Type == "dropdown",
                MultipleChoice = q.Type == "checkbox",
                Options = q.Options?.Select(opt => new Option
                {
                    OptionId = Guid.NewGuid().ToString(),
                    Value = opt
                }).ToList(),
                Format = q.Type == "date_picker" || q.Type == "datepicker" ? "MM/DD/YYYY" : null,
                Required = q.Required,
                Order = index
            }).ToList() ?? new List<Question>();

            // Update FORM-level metadata only
            form.UpdatedAt = DateTime.UtcNow;
            
            _formRepository.Update(form);

            // Return DTO
            var responseDto = new FormLayoutDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description,
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions.Select(q => new QuestionDto
                {
                    Id = q.QuestionId,
                    Text = q.QuestionText,
                    Type = q.Type,
                    Options = q.Options?.Select(o => o.Value).ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.DescriptionEnabled ? q.Description : null
                }).ToList()
            };

            return (true, "Form layout updated successfully", responseDto);
        }


        // -------------------- Common / Utility --------------------
        public (bool Success, string Message) DeleteForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");

            try
            {
                // First, delete all responses from MySQL that are associated with this form
                var responses = _responseRepository.GetByFormId(id);
                int deletedCount = 0;
                
                foreach (var response in responses)
                {
                    _responseRepository.Delete(response.Id.ToString());
                    deletedCount++;
                }

                // Then delete the form from MongoDB
                _formRepository.Delete(id);
                
                return (true, $"Form deleted successfully. {deletedCount} associated responses were also deleted.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting form: {ex.Message}");
            }
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
                    Id = q.QuestionId,
                    Text = q.QuestionText,
                    Type = q.Type,
                    Options = q.Options?.Select(o => o.Value).ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.DescriptionEnabled ? q.Description : null
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
                    Id = q.QuestionId,
                    Text = q.QuestionText,
                    Type = q.Type,
                    Options = q.Options?.Select(o => o.Value).ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.DescriptionEnabled ? q.Description : null
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
