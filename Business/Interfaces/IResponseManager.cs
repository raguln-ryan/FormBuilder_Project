using FormBuilder.API.DTOs.Form;
using System.Security.Claims;
using System.Collections.Generic;
using FormBuilder.API.Models;

namespace FormBuilder.API.Business.Interfaces
{
    public interface IResponseManager
    {
        // Submit a learner's response
        (bool Success, string Message) SubmitResponse(FormSubmissionDto dto, ClaimsPrincipal user);

        // View all responses for a form (Admin)
       IEnumerable<Response> GetResponsesByForm(string formId);

        // View a particular response by ID (Admin)
        (bool Success, string Message, Response? Data) GetResponseById(string responseId);
        
        // Get published forms - returns FormLayoutResponseDto
        List<FormLayoutResponseDto> GetPublishedForms();
    }
}
