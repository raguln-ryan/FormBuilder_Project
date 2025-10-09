using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using System.Security.Claims;
using System.Linq;
using System;
using System.Collections.Generic;
using FormBuilder.API.Common;

namespace FormBuilder.API.Business.Implementations
{
    public class FormManager : IFormManager
    {
        private readonly IFormRepository _formRepository;
        private readonly IQuestionRepository _questionRepository;

        private readonly IResponseRepository _responseRepository;

        public FormManager(IFormRepository formRepository, IQuestionRepository questionRepository, IResponseRepository responseRepository)
        {
            _formRepository = formRepository;
            _questionRepository = questionRepository;
            _responseRepository = responseRepository;
        }

        public (bool Success, string Message, FormLayoutDto? Data) CreateForm(FormLayoutDto dto, string adminUser)
        {
            var form = new Form
            {
                // Id is NOT manually assigned; MongoDB will generate ObjectId
                Title = dto.Title,
                Description = dto.Description,
                Status = (FormStatus)dto.Status,
                Questions = dto.Questions.Select(q => new Question
                {
                    // Id is NOT manually assigned; MongoDB will generate ObjectId
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options,
                    CreatedBy = adminUser,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _formRepository.Add(form);
            return (true, "Form created successfully", MapToDto(form));
        }

        public (bool Success, string Message, FormLayoutDto? Data) UpdateForm(string id, FormLayoutDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);
            if (form.Status == FormStatus.Published)
                return (false, "Cannot edit a published form", null);

            form.Title = dto.Title;
            form.Description = dto.Description;
            form.Status = (FormStatus)dto.Status;

            form.Questions = dto.Questions.Select(q => new Question
            {
                // Id is kept if exists, otherwise MongoDB assigns one
                Id = q.Id ?? null!,
                Text = q.Text,
                Type = q.Type,
                Options = q.Options,
                CreatedBy = q.CreatedBy,
                CreatedAt = q.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            _formRepository.Update(form);
            return (true, "Form updated successfully", MapToDto(form));
        }

        public (bool Success, string Message) DeleteForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");

            try
            {
                _formRepository.DeleteFormAndResponses(id, _responseRepository);
                return (true, "Form and all responses deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public object GetAllForms(ClaimsPrincipal user)
        {
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return role == Roles.Admin ? _formRepository.GetAll() : _formRepository.GetByStatus(FormStatus.Published);
        }

        public (bool Success, string Message, FormLayoutDto? Data) GetFormById(string id, ClaimsPrincipal user)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == Roles.Learner && form.Status != FormStatus.Published)
                return (false, "Access denied", null);

            return (true, "Form retrieved successfully", MapToDto(form));
        }

        public (bool Success, string Message, FormLayoutDto? Data) PreviewForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            return (true, "Preview form", MapToDto(form));
        }

        private FormLayoutDto MapToDto(Form form)
        {
            return new FormLayoutDto
            {
                Title = form.Title,
                Description = form.Description,
                Status = (FormStatusDto)form.Status,
                Questions = form.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options,
                    CreatedBy = q.CreatedBy,
                    CreatedAt = q.CreatedAt ?? DateTime.UtcNow,
                    UpdatedAt = q.UpdatedAt ?? DateTime.UtcNow
                }).ToList()
            };
        }
    }
}
