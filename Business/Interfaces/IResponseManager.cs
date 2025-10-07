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

        // Changed string -> int
        // View all responses for a form (Admin)
        object GetResponsesByForm(int formId);

        // Changed string -> int
        // View a particular response by ID (Admin)
        (bool Success, string Message, object Data) GetResponseById(int responseId);
        
        List<FormDto> GetPublishedForms();
    }
}
