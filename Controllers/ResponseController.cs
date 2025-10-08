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

        [HttpGet("published")]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult GetPublishedForms()
        {
            var forms = _responseManager.GetPublishedForms();
            return Ok(forms);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Learner)]
        public IActionResult SubmitResponse([FromBody] FormSubmissionDto dto)
        {
            var result = _responseManager.SubmitResponse(dto, User);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet("form/{formId}/responses")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponsesByForm(string formId)
        {
            var result = _responseManager.GetResponsesByForm(formId);
            return Ok(result);
        }

        [HttpGet("{responseId}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult GetResponseById(string responseId)
        {
            var result = _responseManager.GetResponseById(responseId);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }
    }
}
