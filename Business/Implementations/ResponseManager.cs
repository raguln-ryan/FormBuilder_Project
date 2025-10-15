using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Models;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormBuilder.API.Business.Implementations
{
    public class ResponseManager : IResponseManager
    {
        private readonly IResponseRepository _responseRepository;
        private readonly IFormRepository _formRepository;

        public ResponseManager(IResponseRepository responseRepository, IFormRepository formRepository)
        {
            _responseRepository = responseRepository;
            _formRepository = formRepository;
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

        public IEnumerable<Response> GetResponsesByForm(string formId) =>
            _responseRepository.GetByFormId(formId);

        public (bool Success, string Message, Response? Data) GetResponseById(string responseId)
        {
            var response = _responseRepository.GetById(responseId);
            return response == null
                ? (false, "Response not found", null)
                : (true, "Response retrieved successfully", response);
        }

        public (bool Success, string Message) SubmitResponse(FormSubmissionDto dto, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? user.FindFirst("nameId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                return (false, "Invalid user ID.");

            if (string.IsNullOrEmpty(dto.FormId))
                return (false, "Form ID is required.");

            var form = _formRepository.GetById(dto.FormId);
            if (form == null)
                return (false, "Invalid form ID.");

            if (form.Status != FormStatus.Published)
                return (false, "Cannot submit to an unpublished form.");

            if (dto.Answers == null || !dto.Answers.Any())
                return (false, "No answers provided.");

            // Validate that all required questions are answered
            var requiredQuestions = form.Questions.Where(q => q.Required).ToList();
            foreach (var question in requiredQuestions)
            {
                var answer = dto.Answers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                if (answer == null || string.IsNullOrWhiteSpace(answer.Answer))
                {
                    return (false, $"Question '{question.QuestionText}' is required.");
                }
            }

            // Process and format answers based on question type
            var responseDetails = dto.Answers.Select(a =>
            {
                var question = form.Questions.FirstOrDefault(q => q.QuestionId == a.QuestionId);
                var formattedAnswer = a.Answer ?? string.Empty;

                // Check if the question has options (checkbox, radio, dropdown, etc.)
                if (question != null && question.Options != null && question.Options.Any())
                {
                    // Debug: Log what we have in Options
                    System.Diagnostics.Debug.WriteLine($"Question: {question.QuestionText}");
                    System.Diagnostics.Debug.WriteLine($"Options count: {question.Options.Count}");
                    foreach (var opt in question.Options)
                    {
                        System.Diagnostics.Debug.WriteLine($"Option - ID: {opt.OptionId}, Value: {opt.Value}");
                    }

                    // Check if it's a multiple choice question (checkbox type)
                    if (question.Type.ToLower() == "checkbox" || question.MultipleChoice)
                    {
                        // Parse the answer as multiple selections (comma-separated values)
                        var selectedValues = a.Answer?.Split(',')
                            .Select(val => val.Trim())
                            .Where(val => !string.IsNullOrEmpty(val))
                            .ToList();

                        if (selectedValues != null && selectedValues.Any())
                        {
                            // Map selected values to their actual option IDs
                            var selectedOptionIds = new List<string>();
                            foreach (var value in selectedValues)
                            {
                                var option = question.Options.FirstOrDefault(o => o.Value == value);
                                if (option != null && !string.IsNullOrEmpty(option.OptionId))
                                {
                                    // Use the actual OptionId with quotes
                                    selectedOptionIds.Add($"\"{option.OptionId}\"");
                                    System.Diagnostics.Debug.WriteLine($"Matched '{value}' to ID: {option.OptionId}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"Could not find option for value: {value}");
                                }
                            }

                            // Format as ["actualId1","actualId2","actualId3"]
                            if (selectedOptionIds.Any())
                            {
                                formattedAnswer = $"[{string.Join(",", selectedOptionIds)}]";
                            }
                        }
                    }
                    else if (question.Type.ToLower() == "radio" || question.Type.ToLower() == "dropdown")
                    {
                        // For single selection, find the option ID for the selected value
                        var selectedOption = question.Options.FirstOrDefault(o => o.Value == a.Answer);
                        if (selectedOption != null && !string.IsNullOrEmpty(selectedOption.OptionId))
                        {
                            // Format as ["actualId"] with quotes
                            formattedAnswer = $"[\"{selectedOption.OptionId}\"]";
                            System.Diagnostics.Debug.WriteLine($"Single selection - Matched '{a.Answer}' to ID: {selectedOption.OptionId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Could not find option for single value: {a.Answer}");
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Final formatted answer: {formattedAnswer}");
                
                return new ResponseDetail
                {
                    QuestionId = a.QuestionId,
                    Answer = formattedAnswer
                };
            }).ToList();

            var response = new Response
            {
                FormId = dto.FormId,
                UserId = userId,
                SubmittedAt = DateTime.UtcNow,
                Details = responseDetails
            };

            try
            {
                _responseRepository.Add(response);
                return (true, "Response submitted successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
