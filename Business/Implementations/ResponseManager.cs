using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Models;
using System.Security.Claims;
using System.Linq;
using System;
using System.Collections.Generic;

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
                    Title = f.Title,
                    Description = f.Description,
                    Status = (FormStatusDto)f.Status,
                    Questions = f.Questions.Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Type = q.Type,
                        Options = q.Options,
                        CreatedBy = q.CreatedBy,
                        CreatedAt = q.CreatedAt ?? DateTime.UtcNow,
                        UpdatedAt = q.UpdatedAt ?? DateTime.UtcNow
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
            // 1️⃣ Extract UserId from JWT
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? user.FindFirst("nameId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId <= 0)
                return (false, "Invalid user ID.");

            // 2️⃣ Validate form exists
            var form = _formRepository.GetById(dto.FormId);
            if (form == null)
                return (false, "Invalid form ID.");

            // 3️⃣ Validate form is published
            if (form.Status != FormStatus.Published)
                return (false, "Cannot submit to an unpublished form.");

            // 4️⃣ Validate answers
            if (dto.Answers == null || !dto.Answers.Any())
                return (false, "No answers provided.");
            var responseDetails = dto.Answers.Select(a => new ResponseDetail
            {
                QuestionId = a.QuestionId,
                Answer = a.Answer
            }).ToList();

            // ✅ 5️⃣ Map answers to ResponseDetails and create Response
            var response = new Response
            {
                FormId = dto.FormId,
                UserId = userId,
                SubmittedAt = DateTime.UtcNow,
                Details = responseDetails
               
            };
            

            // 6️⃣ Save to database
            try
            {
                _responseRepository.Add(response); // EF handles IDs automatically
                return (true, "Response submitted successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }


    }
}
