using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Common;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseManager _responseManager;

        public ResponseController(IResponseManager responseManager)
        {
            _responseManager = responseManager;
        }

        [HttpGet("published")]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult GetPublishedForms()
        {
            var forms = _responseManager.GetPublishedForms();
            return Ok(forms);
        }

        // NEW ENDPOINT - Get specific form for submission
        [HttpGet("form/{formId}")]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult GetFormForSubmission(string formId)
        {
            var form = _responseManager.GetFormById(formId);
            
            if (form == null)
                return NotFound(new { success = false, message = "Form not found or not published" });
                
            return Ok(form);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult SubmitResponse([FromBody] FormSubmissionDto dto)
        {
            var result = _responseManager.SubmitResponse(dto, User);
            
            if (!result.Success) 
                return BadRequest(new { success = false, message = result.Message });
                
            return Ok(new { 
                success = true, 
                message = result.Message,
                responseId = result.Data?.Id 
            });
        }

        [HttpGet("form/{formId}/responses")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponsesByForm(string formId)
        {
            var responses = _responseManager.GetResponsesByForm(formId);
            
            // Format the response to include user details
            var formattedResponses = responses.Select(r => new
            {
                id = r.Id,
                formId = r.FormId,
                userId = r.UserId,
                submittedAt = r.SubmittedAt,
                details = r.Details,
                user = r.User != null ? new
                {
                    id = r.User.Id,
                    name = r.User.Name,
                    email = r.User.Email
                } : null
            });
            
            return Ok(formattedResponses);
        }

        [HttpGet("{responseId}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponseById(string responseId)
        {
            var result = _responseManager.GetResponseById(responseId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpGet("{responseId}/file/{questionId}")]
        [Authorize]
        public IActionResult DownloadFile(string responseId, string questionId)
        {
            if (!int.TryParse(responseId, out int id))
                return BadRequest(new { success = false, message = "Invalid response ID" });

            var result = _responseManager.GetFileAttachment(id, questionId);
            
            if (!result.Success)
                return NotFound(new { success = false, message = result.Message });

            var file = result.Data;
            
            var bytes = Convert.FromBase64String(file.Base64Content);
            return File(bytes, file.FileType, file.FileName);
        }

        [HttpGet("{responseId}/details")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponseWithDetails(string responseId)
        {
            if (!int.TryParse(responseId, out int id))
                return BadRequest(new { success = false, message = "Invalid response ID" });

            var result = _responseManager.GetResponseWithFiles(id);
            
            if (!result.Success)
                return NotFound(new { success = false, message = result.Message });

            return Ok(result.Data);
        }
    }
}
