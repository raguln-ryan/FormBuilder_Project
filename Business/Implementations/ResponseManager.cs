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

        public List<FormLayoutDto> GetPublishedForms()
        {
            return _formRepository
                .GetByStatus(FormStatus.Published)
                .Select(f => new FormLayoutDto
                {
                    FormId = f.Id,
    Title = f.Title,
    Description = f.Description,
    Status = (FormStatusDto)f.Status,
    Questions = f.Questions.Select(q => new QuestionDto
    {
        Id = q.Id,
        Text = q.Text,
        Type = q.Type,
        Options = q.Options?.ToArray() ?? Array.Empty<string>()
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

            var responseDetails = dto.Answers.Select(a => new ResponseDetail
            {
                QuestionId = a.QuestionId,
                Answer = a.Answer ?? string.Empty
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
