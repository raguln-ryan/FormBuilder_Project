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
                UpdatedAt = DateTime.UtcNow
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

        // -------------------- Form Layout --------------------
        public (bool Success, string Message, FormLayoutResponseDto? Data) CreateFormLayout(FormLayoutRequestDto dto, string adminUser)
        {
            var form = _formRepository.GetById(dto.FormId);
            if (form == null) return (false, "Form not found", null);

            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot modify layout of a published form.", null);
            }

            // Generate proper ObjectIds for questions and options
            form.Questions = dto.Questions?.Select((q, index) => 
            {
                var question = new Question
                {
                    QuestionId = ObjectId.GenerateNewId().ToString(), // Generates proper 24-char hex ID
                    QuestionText = q.Text,
                    Type = q.Type,
                    DescriptionEnabled = q.DescriptionEnabled,
                    Description = q.Description ?? "",
                    SingleChoice = q.SingleChoice,
                    MultipleChoice = q.MultipleChoice,
                    Format = q.Format,
                    Required = q.Required,
                    Order = q.Order
                };

                // Generate proper ObjectIds for each option
                if (q.Options != null && q.Options.Any())
                {
                    question.Options = q.Options.Select(optValue => new Option
                    {
                        OptionId = ObjectId.GenerateNewId().ToString(), // Generates proper 24-char hex ID
                        Value = optValue
                    }).ToList();
                }
                else
                {
                    question.Options = new List<Option>();
                }

                return question;
            }).ToList() ?? new List<Question>();

            form.UpdatedAt = DateTime.UtcNow;
            form.UpdatedBy = adminUser;
            
            _formRepository.Update(form);

            // Log generated IDs for debugging
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

            return (true, "Form layout created successfully", new FormLayoutResponseDto
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
                    DescriptionEnabled = q.DescriptionEnabled,
                    SingleChoice = q.SingleChoice,
                    MultipleChoice = q.MultipleChoice,
                    Format = q.Format,
                    Order = q.Order
                }).ToList()
            });
        }

        public (bool Success, string Message, FormLayoutResponseDto? Data) UpdateFormLayout(string id, FormLayoutRequestDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);
            
            if (form.Status == FormStatus.Published)
            {
                return (false, "Cannot update layout of a published form.", null);
            }

            form.Questions = dto.Questions?.Select((q, index) =>
            {
                var question = new Question
                {
                    // Use existing ID if provided and valid, otherwise generate new one
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
                    Order = q.Order
                };

                // Always generate new ObjectIds for options
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

            form.UpdatedAt = DateTime.UtcNow;
            
            _formRepository.Update(form);

            return (true, "Form layout updated successfully", new FormLayoutResponseDto
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
                    DescriptionEnabled = q.DescriptionEnabled,
                    SingleChoice = q.SingleChoice,
                    MultipleChoice = q.MultipleChoice,
                    Format = q.Format,
                    Order = q.Order
                }).ToList()
            });
        }


        // -------------------- Common / Utility --------------------
        public (bool Success, string Message) DeleteForm(string id)
        {
            try
            {
                var form = _formRepository.GetById(id);
                if (form == null) 
                    return (false, "Form not found");

                var responses = _responseRepository.GetByFormId(id);
                var responseCount = responses.Count();
                
                var deletedResponses = 0;
                var failedDeletions = new List<string>();
                
                foreach (var response in responses)
                {
                    try
                    {
                        _responseRepository.Delete(response.Id.ToString());
                        deletedResponses++;
                    }
                    catch (Exception ex)
                    {
                        failedDeletions.Add($"ResponseId: {response.Id} - Error: {ex.Message}");
                    }
                }

                if (failedDeletions.Any())
                {
                    return (false, $"Failed to delete some responses. Deleted {deletedResponses}/{responseCount} responses. " +
                                   $"Errors: {string.Join("; ", failedDeletions)}. Form not deleted.");
                }

                _formRepository.Delete(id);
                
                return (true, $"Form and {deletedResponses} associated response(s) deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting form: {ex.Message}");
            }
        }

        public object GetAllForms(ClaimsPrincipal user)
        {
            var forms = _formRepository.GetAll();
            return forms.Select(f => new FormLayoutResponseDto
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
