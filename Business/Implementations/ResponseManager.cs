using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.API.Business.Implementations
{
    public class ResponseManager : IResponseManager
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFileAttachmentRepository _fileAttachmentRepository;
        private readonly MySqlDbContext _dbContext;

        public ResponseManager(IResponseRepository responseRepository, IFormRepository formRepository,
            IFileAttachmentRepository fileAttachmentRepository,
            MySqlDbContext dbContext)
        {
            _responseRepository = responseRepository;
            _formRepository = formRepository;
            _fileAttachmentRepository = fileAttachmentRepository;
            _dbContext = dbContext;
        }

        public List<FormLayoutResponseDto> GetPublishedForms()
        {
            return _formRepository
                .GetByStatus(FormStatus.Published)
                .Select(f => new FormLayoutResponseDto
                {
                    FormId = f.Id,
                    Title = f.Title,
                    Description = f.Description,
                    Status = (FormStatusDto)f.Status,
                    Questions = f.Questions.Select(q => new QuestionDto
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
                    }).ToList()
                }).ToList();
        }

        // NEW METHOD - Get specific form by ID
        public FormLayoutResponseDto GetFormById(string formId)
        {
            var form = _formRepository.GetById(formId);
            
            // Check if form exists and is published
            if (form == null || form.Status != FormStatus.Published)
                return null;
            
            return new FormLayoutResponseDto
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
        }

        public IEnumerable<Response> GetResponsesByForm(string formId) =>
            _responseRepository.GetByFormId(formId);

        public (bool Success, string Message, Response? Data) GetResponseById(string responseId)
        {
            var response = _responseRepository.GetById(responseId);
            return response == null
                ? (false, "Response not found", null)
                : (true, "Response retrieved successfully", response);
        }

        public (bool Success, string Message, Response? Data) SubmitResponse(FormSubmissionDto dto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? user.FindFirst("nameId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                return (false, "Invalid user ID.", null);

            if (string.IsNullOrEmpty(dto.FormId))
                return (false, "Form ID is required.", null);

            var form = _formRepository.GetById(dto.FormId);
            if (form == null)
                return (false, "Invalid form ID.", null);

            if (form.Status != FormStatus.Published)
                return (false, "Cannot submit to an unpublished form.", null);

            // Validate file uploads if present
            if (dto.FileUploads != null && dto.FileUploads.Any())
            {
                const long maxFileSizeInBytes = 5 * 1024 * 1024; // 5MB
                var allowedTypes = new[] { 
                    "image/jpeg", "image/jpg", "image/png", "image/gif", 
                    "application/pdf", "application/msword", 
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document" 
                };

                foreach (var file in dto.FileUploads)
                {
                    if (file.FileSize > maxFileSizeInBytes)
                        return (false, $"File {file.FileName} exceeds maximum size of 5MB", null);

                    if (!allowedTypes.Contains(file.FileType.ToLower()))
                        return (false, $"File type {file.FileType} is not allowed for {file.FileName}", null);
                }
            }

            // Validate that all required questions are answered
            var requiredQuestions = form.Questions.Where(q => q.Required).ToList();
            foreach (var question in requiredQuestions)
            {
                // Check if it's a file upload question
                if (question.Type.ToLower() == "file" || question.Type.ToLower() == "file_upload")
                {
                    var hasFile = dto.FileUploads?.Any(f => f.QuestionId == question.QuestionId) ?? false;
                    if (!hasFile)
                    {
                        return (false, $"File upload for '{question.QuestionText}' is required.", null);
                    }
                }
                else
                {
                    var answer = dto.Answers?.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                    if (answer == null || string.IsNullOrWhiteSpace(answer.Answer))
                    {
                        return (false, $"Question '{question.QuestionText}' is required.", null);
                    }
                }
            }

            // Process and format answers based on question type
            var responseDetails = new List<ResponseDetail>();
            
            if (dto.Answers != null)
            {
                responseDetails = dto.Answers.Select(a =>
                {
                    var question = form.Questions.FirstOrDefault(q => q.QuestionId == a.QuestionId);
                    var formattedAnswer = a.Answer ?? string.Empty;

                    // Check if the question has options (checkbox, radio, dropdown, etc.)
                    if (question != null && question.Options != null && question.Options.Any())
                    {
                        // Check if it's a multiple choice question (checkbox type)
                        if (question.Type.ToLower() == "checkbox" || question.MultipleChoice)
                        {
                            var selectedValues = a.Answer?.Split(',')
                                .Select(val => val.Trim())
                                .Where(val => !string.IsNullOrEmpty(val))
                                .ToList();

                            if (selectedValues != null && selectedValues.Any())
                            {
                                var selectedOptionIds = new List<string>();
                                foreach (var value in selectedValues)
                                {
                                    var option = question.Options.FirstOrDefault(o => o.Value == value);
                                    if (option != null && !string.IsNullOrEmpty(option.OptionId))
                                    {
                                        selectedOptionIds.Add($"\"{option.OptionId}\"");
                                    }
                                }

                                if (selectedOptionIds.Any())
                                {
                                    formattedAnswer = $"[{string.Join(",", selectedOptionIds)}]";
                                }
                            }
                        }
                        else if (question.Type.ToLower() == "radio" || question.Type.ToLower() == "dropdown" || question.SingleChoice)
                        {
                            var selectedOption = question.Options.FirstOrDefault(o => o.Value == a.Answer);
                            if (selectedOption != null && !string.IsNullOrEmpty(selectedOption.OptionId))
                            {
                                formattedAnswer = $"[\"{selectedOption.OptionId}\"]";
                            }
                        }
                    }
                    
                    return new ResponseDetail
                    {
                        QuestionId = a.QuestionId,
                        Answer = formattedAnswer
                    };
                }).ToList();
            }

            // Add file references to response details
            if (dto.FileUploads != null && dto.FileUploads.Any())
            {
                foreach (var file in dto.FileUploads.GroupBy(f => f.QuestionId))
                {
                    // Only add if not already in response details
                    if (!responseDetails.Any(d => d.QuestionId == file.Key))
                    {
                        responseDetails.Add(new ResponseDetail
                        {
                            QuestionId = file.Key,
                            Answer = $"[FILE_UPLOADED:{string.Join(",", file.Select(f => f.FileName))}]"
                        });
                    }
                }
            }

            var response = new Response
            {
                FormId = dto.FormId,
                UserId = userId,
                SubmittedAt = DateTime.UtcNow,
                Details = responseDetails
            };

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Save response first
                    _responseRepository.Add(response);

                    // Save file attachments to MySQL
                    if (dto.FileUploads != null && dto.FileUploads.Any())
                    {
                        var fileAttachments = dto.FileUploads.Select(file => new FileAttachment
                        {
                            ResponseId = response.Id,
                            QuestionId = file.QuestionId,
                            FileName = file.FileName,
                            FileType = file.FileType,
                            FileSize = file.FileSize,
                            Base64Content = file.Base64Content,
                            UploadedAt = DateTime.UtcNow
                        }).ToList();

                        _fileAttachmentRepository.AddRange(fileAttachments);
                        _fileAttachmentRepository.SaveChanges();
                    }

                    transaction.Commit();
                    return (true, "Response submitted successfully", response);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    
                    // Try to delete the response if it was saved
                    if (response.Id > 0)
                    {
                        try
                        {
                            _responseRepository.Delete(response.Id.ToString());
                        }
                        catch { /* Log error */ }
                    }

                    return (false, $"Error: {ex.InnerException?.Message ?? ex.Message}", null);
                }
            }
        }

        public (bool Success, string Message, FileAttachment? Data) GetFileAttachment(int responseId, string questionId)
        {
            var file = _fileAttachmentRepository.GetByResponseAndQuestion(responseId, questionId);

            if (file == null)
                return (false, "File not found", null);

            return (true, "File retrieved successfully", file);
        }

        public (bool Success, string Message, object? Data) GetResponseWithFiles(int responseId)
        {
            var response = _responseRepository.GetById(responseId.ToString());
            if (response == null)
                return (false, "Response not found", null);

            var files = _fileAttachmentRepository.GetByResponseId(responseId)
                .Select(f => new
                {
                    f.Id,
                    f.QuestionId,
                    f.FileName,
                    f.FileType,
                    f.FileSize,
                    f.UploadedAt
                })
                .ToList();

            return (true, "Response retrieved successfully", new
            {
                Response = response,
                FileAttachments = files
            });
        }

        public IEnumerable<object> GetUserSubmissions(int userId)
        {
            // Get all responses for this user
            var userResponses = _responseRepository.GetByUserId(userId);
            
            var result = new List<object>();
            
            foreach (var response in userResponses)
            {
                // Get form details for each response
                var form = _formRepository.GetById(response.FormId);
                
                result.Add(new
                {
                    id = response.Id,
                    formId = response.FormId,
                    formTitle = form?.Title ?? "Unknown Form",
                    formDescription = form?.Description ?? "",
                    submittedAt = response.SubmittedAt,
                    status = "submitted", // You can add actual status logic here
                    userId = response.UserId,
                    questionCount = form?.Questions?.Count ?? 0,
                    details = response.Details?.Select(d => new
                    {
                        questionId = d.QuestionId,
                        answer = d.Answer
                    }).ToList()
                });
            }
            
            return result.OrderByDescending(r => ((dynamic)r).submittedAt);
        }
    }
}
