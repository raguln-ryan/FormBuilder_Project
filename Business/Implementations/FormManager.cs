using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using System.Security.Claims;
using FormBuilder.API.Common;

namespace FormBuilder.API.Business.Implementations
{
    public class FormManager : IFormManager
    {
        private readonly IFormRepository _formRepository;
        private readonly IQuestionRepository _questionRepository;

        public FormManager(IFormRepository formRepository, IQuestionRepository questionRepository)
        {
            _formRepository = formRepository;
            _questionRepository = questionRepository;
        }

        public (bool Success, string Message, FormLayoutDto Data) CreateForm(FormLayoutDto dto)
        {
            var form = new Form
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = FormStatus.Draft,
                Questions = dto.Questions
            };

            _formRepository.Add(form);

            return (true, "Form created successfully", dto);
        }

        public (bool Success, string Message, FormLayoutDto Data) UpdateForm(string id, FormLayoutDto dto)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            form.Title = dto.Title;
            form.Description = dto.Description;
            form.Questions = dto.Questions;

            _formRepository.Update(form);

            return (true, "Form updated successfully", dto);
        }

        public (bool Success, string Message) DeleteForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");

            _formRepository.Delete(id);

            return (true, "Form deleted successfully");
        }

        public object GetAllForms(ClaimsPrincipal user)
        {
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == Roles.Admin)
                return _formRepository.GetAll();
            else
                return _formRepository.GetByStatus(FormStatus.Published);
        }

        public (bool Success, string Message, FormLayoutDto Data) GetFormById(string id, ClaimsPrincipal user)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == Roles.Learner && form.Status != FormStatus.Published)
                return (false, "Access denied", null);

            return (true, "Form retrieved successfully", new FormLayoutDto
            {
                Title = form.Title,
                Description = form.Description,
                Questions = form.Questions
            });
        }

        public (bool Success, string Message) PublishForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");

            form.Status = FormStatus.Published;
            _formRepository.Update(form);

            return (true, "Form published successfully");
        }

        public (bool Success, string Message) DraftForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found");

            form.Status = FormStatus.Draft;
            _formRepository.Update(form);

            return (true, "Form moved to draft");
        }

        public (bool Success, string Message, FormLayoutDto Data) PreviewForm(string id)
        {
            var form = _formRepository.GetById(id);
            if (form == null) return (false, "Form not found", null);

            return (true, "Preview form", new FormLayoutDto
            {
                Title = form.Title,
                Description = form.Description,
                Questions = form.Questions
            });
        }
    }
}
