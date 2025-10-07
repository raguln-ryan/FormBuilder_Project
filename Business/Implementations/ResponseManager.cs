using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.DTOs.Form;
using System.Security.Claims;
using FormBuilder.API.Business.Interfaces;

namespace FormBuilder.API.Business.Implementations
{
    public class ResponseManager : IResponseManager
    {
        private readonly IResponseRepository _repo;
        private readonly IFormRepository _formRepo;

        public ResponseManager(IResponseRepository repo, IFormRepository formRepo)
        {
            _repo = repo;
            _formRepo = formRepo;
        }

        public object GetResponsesByForm(int formId)
        {
            return _repo.GetByFormId(formId);
        }

        public (bool Success, string Message, object Data) GetResponseById(int responseId)
        {
            var response = _repo.GetById(responseId);
            if (response == null)
                return (false, "Response not found", null);

            return (true, "Response retrieved", response);
        }

        public (bool Success, string Message) SubmitResponse(FormSubmissionDto dto, ClaimsPrincipal user)
        {
            // Example: map DTO to Response entity and save
            var response = new Response
            {
                FormId = dto.FormId,
                UserId = int.Parse(user.FindFirst("Id")?.Value ?? "0"), // Assuming Claim "Id" is int
                SubmittedAt = DateTime.UtcNow,
                Details = dto.Answers.Select(a => new ResponseDetail
                {
                    QuestionId = a.QuestionId,
                    Answer = a.Answer
                }).ToList()
            };

            _repo.Add(response);

            return (true, "Submitted successfully");
        }

        // NEW: Get all published forms (Learner)
      // NEW: Get all published forms (Learner)
        public List<FormDto> GetPublishedForms()
        {
            var forms = _formRepo.GetByStatus(FormStatus.Published)
                                 .Select(f => new FormDto
                                 {
                                     Id = f.Id,
                                     Title = f.Title,
                                     Description = f.Description,
                                     Status = (int)f.Status,
                                     Questions = f.Questions.Select(q => new QuestionDto
                                     {
                                         Id = q.Id,
                                         Text = q.Text,
                                         Type = q.Type,
                                         Options = q.Options,
                                         CreatedAt = q.CreatedAt,
                                         UpdatedAt = q.UpdatedAt
                                     }).ToList()
                                 })
                                 .ToList();
            return forms;
        }
    }
}
