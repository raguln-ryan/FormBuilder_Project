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
        public (bool Success, string Message, FormConfigResponseDto? Data) CreateFormConfig(FormConfigRequestDto dto, string adminUser)
        {
            var form = new Form
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = FormStatus.Draft,
                CreatedBy = adminUser,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Questions = new List<Question>() // Initialize with empty questions
            };

            _formRepository.Add(form);

            return (true, "Form configuration created successfully", new FormConfigResponseDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description
            });
        }

        public (bool Success, string Message, FormConfigResponseDto? Data) UpdateFormConfig(string id, FormConfigRequestDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot update configuration of a published form.", null);
            }

            form.Title = dto.Title;
            form.Description = dto.Description;
            form.UpdatedAt = DateTime.UtcNow;

            _formRepository.Update(form);

            return (true, "Form configuration updated successfully", new FormConfigResponseDto
            {
                FormId = form.Id,
                Title = form.Title,
                Description = form.Description
            });
        }

        // -------------------- Form Layout - Only Updates Existing Forms --------------------
        public (bool Success, string Message, FormLayoutResponseDto? Data) UpdateFormLayout(string formId, FormLayoutRequestDto dto, string adminUser)
        {
            // Step 1: Check if form exists
            var form = _formRepository.GetById(formId);
            if (form == null)
            {
                return (false, "Form not found. Please create form configuration first using the FormConfig endpoint.", null);
            }

            // Step 2: Check if form is published
            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot modify layout of a published form.", null);
            }

            // Check if this is a create or update operation
            bool isCreateOperation = form.Questions == null || !form.Questions.Any();

            // Step 3: Update only the questions (not title/description)
            form.Questions = dto.Questions?.Select((q, index) =>
            {
                var question = new Question
                {
                    // Preserve existing QuestionId if valid, otherwise generate new
                    QuestionId = (!string.IsNullOrEmpty(q.Id) && q.Id != "000000000000000000000000")
                        ? q.Id
                        : ObjectId.GenerateNewId().ToString(),
                    QuestionText = q.Text,
                    Type = q.Type,
                    DescriptionEnabled = q.DescriptionEnabled,
                    Description = q.Description ?? "",
                    SingleChoice = q.SingleChoice,
                    MultipleChoice = q.MultipleChoice,
                    Format = q.Format,
                    Required = q.Required,
                    Order = q.Order > 0 ? q.Order : index  // Fixed: Use provided Order if > 0, otherwise use index
                };

                // Generate ObjectIds for options
                if (q.Options != null && q.Options.Any())
                {
                    question.Options = q.Options.Select(optValue => new Option
                    {
                        OptionId = ObjectId.GenerateNewId().ToString(),
                        Value = optValue
                    }).ToList();
                }
                else
                {
                    question.Options = new List<Option>();
                }

                return question;
            }).ToList() ?? new List<Question>();

            // Step 4: Update metadata
            form.UpdatedAt = DateTime.UtcNow;
            form.UpdatedBy = adminUser;

            // Step 5: Save to repository
            _formRepository.Update(form);

            // Log for debugging
            System.Diagnostics.Debug.WriteLine($"Form layout {(isCreateOperation ? "created" : "updated")}: {form.Id}");
            foreach (var q in form.Questions)
            {
                System.Diagnostics.Debug.WriteLine($"Question ID: {q.QuestionId}, Text: {q.QuestionText}");
                if (q.Options != null)
                {
                    foreach (var opt in q.Options)
                    {
                        System.Diagnostics.Debug.WriteLine($"  Option ID: {opt.OptionId}, Value: {opt.Value}");
                    }
                }
            }

            // Step 6: Return response with full form data (including title/description from DB)
            var responseDto = new FormLayoutResponseDto
            {
                FormId = form.Id,
                Title = form.Title,                    // From database, not from DTO
                Description = form.Description,        // From database, not from DTO
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions.Select(q => new QuestionDto
                {
                    Id = q.QuestionId,
                    Text = q.QuestionText,
                    Type = q.Type,
                    Options = q.Options?.Select(o => o.Value).ToArray() ?? Array.Empty<string>(),
                    Required = q.Required,
                    Description = q.Description,
                    DescriptionEnabled = q.DescriptionEnabled,
                    SingleChoice = q.SingleChoice,
                    MultipleChoice = q.MultipleChoice,
                    Format = q.Format,
                    Order = q.Order
                }).ToList()
            };

            // Return appropriate success message based on operation type
            string successMessage = isCreateOperation 
                ? "Form layout created successfully" 
                : "Form layout updated successfully";

            return (true, successMessage, responseDto);
        }

        // -------------------- Common / Utility --------------------
        public (bool Success, string Message) DeleteForm(string id)
        {
            try
            {
                var form = _formRepository.GetById(id);
                if (form == null) 
                    return (false, "Form not found");

                // Delete all responses in one operation
                var deletedResponseCount = _responseRepository.DeleteAllByFormId(id);
        
                // Now delete the form
                _formRepository.Delete(id);
        
                return (true, $"Form and {deletedResponseCount} response(s) deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting form: {ex.Message}");
            }
        }

        public (bool Success, string Message, object Data) GetAllForms(ClaimsPrincipal user, int offset = 0, int limit = 10)
        {
            try
            {
                // Validate pagination parameters
                if (offset < 0) offset = 0;
                if (limit <= 0) limit = 10;
                if (limit > 100) limit = 100; // Maximum limit to prevent excessive data retrieval

                // Get all forms from repository
                var allForms = _formRepository.GetAll();
                
                // Get total count before pagination
                var totalCount = allForms.Count();

                // Apply pagination
                var paginatedForms = allForms
                    .Skip(offset)
                    .Take(limit)
                    .Select(f => new FormLayoutResponseDto
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
                            Description = q.DescriptionEnabled ? q.Description : null,
                            DescriptionEnabled = q.DescriptionEnabled,
                            SingleChoice = q.SingleChoice,
                            MultipleChoice = q.MultipleChoice,
                            Format = q.Format,
                            Order = q.Order
                        }).ToList() ?? new List<QuestionDto>()
                    }).ToList();

                // Create paginated response
                var paginatedResponse = new
                {
                    data = paginatedForms,
                    pagination = new
                    {
                        offset = offset,
                        limit = limit,
                        total = totalCount,
                        hasNext = (offset + limit) < totalCount,
                        hasPrevious = offset > 0,
                        nextOffset = (offset + limit) < totalCount ? offset + limit : (int?)null,
                        previousOffset = offset - limit >= 0 ? offset - limit : (int?)null
                    }
                };

                return (true, "Forms retrieved successfully", paginatedResponse);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving forms: {ex.Message}", null);
            }
        }

        public (bool Success, string Message, FormLayoutResponseDto? Data) GetFormById(string id, ClaimsPrincipal user)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            var dto = new FormLayoutResponseDto
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
                    Description = q.DescriptionEnabled ? q.Description : null,
                    DescriptionEnabled = q.DescriptionEnabled,
                    SingleChoice = q.SingleChoice,
                    MultipleChoice = q.MultipleChoice,
                    Format = q.Format,
                    Order = q.Order
                }).ToList() ?? new List<QuestionDto>()
            };

            return (true, "Form retrieved successfully", dto);
        }

        public (bool Success, string Message) PublishForm(string id, string publishedBy)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");
            
            if (form.Questions == null || !form.Questions.Any())
            {
                return (false, "Cannot publish a form without questions. Please add questions using the Layout endpoint first.");
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
