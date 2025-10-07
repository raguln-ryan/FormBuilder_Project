using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Common;
using Microsoft.AspNetCore.Authorization;

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
        // GET: api/Response/published
        [HttpGet("published")]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult GetPublishedForms()
        {
            // This method should fetch only forms where Status = Published
            var forms = _responseManager.GetPublishedForms(); // You need to implement this in your manager
            return Ok(forms);
        }

        // Submit answers (Learner only)
        [HttpPost]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult SubmitResponse([FromBody] FormSubmissionDto dto)
        {
            var result = _responseManager.SubmitResponse(dto, User);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        // View all responses for a form (Admin only)
        [HttpGet("form/{formId}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponsesByForm(int formId)  // <-- changed to int
        {
            var result = _responseManager.GetResponsesByForm(formId);
            return Ok(result);
        }

        // View a particular response by its ID (Admin only)
        [HttpGet("{responseId}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponseById(int responseId)  // <-- changed to int
        {
            var result = _responseManager.GetResponseById(responseId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }
    }
}
