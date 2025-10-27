using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace FormBuilder.API.Business.Interfaces
{
        public interface IResponseManager
        {
            List<FormLayoutResponseDto> GetPublishedForms();
            FormLayoutResponseDto GetFormById(string formId);
            IEnumerable<Response> GetResponsesByForm(string formId);
            (bool Success, string Message, Response? Data) GetResponseById(string responseId);
            (bool Success, string Message, Response? Data) SubmitResponse(FormSubmissionDto dto, ClaimsPrincipal user);
            (bool Success, string Message, FileAttachment? Data) GetFileAttachment(int responseId, string questionId);
            (bool Success, string Message, object? Data) GetResponseWithFiles(int responseId);
        }
}
