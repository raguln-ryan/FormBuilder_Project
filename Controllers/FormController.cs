using Microsoft.AspNetCore.Mvc;
using FormBuilder.API.Business.Interfaces;
using FormBuilder.API.DTOs.Form;
using FormBuilder.API.Common;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly IFormManager _formManager;

        public FormController(IFormManager formManager)
        {
            _formManager = formManager;
        }

        // -------------------- Form Configuration --------------------
        [HttpPost("FormConfig")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreateFormConfig([FromBody] FormConfigDto dto)
        {
            var adminUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
            var result = _formManager.CreateFormConfig(dto, adminUser);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("FormConfig/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult UpdateFormConfig(string id, [FromBody] FormConfigDto dto)
        {
            var result = _formManager.UpdateFormConfig(id, dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        // -------------------- Form Layout --------------------
        [HttpPost("Layout")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult CreateFormLayout([FromBody] FormLayoutDto dto)
        {
            var adminUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
            var result = _formManager.CreateFormLayout(dto, adminUser);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        [HttpPut("Layout/{id}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult UpdateFormLayout(string id, [FromBody] FormLayoutDto dto)
        {
            var result = _formManager.UpdateFormLayout(id, dto);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Data);
        }

        // -------------------- Common / Utility --------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult DeleteForm(string id)
        {
            var result = _formManager.DeleteForm(id);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllForms()
        {
            var result = _formManager.GetAllForms(User);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetFormById(string id)
        {
            var result = _formManager.GetFormById(id, User);
            if (!result.Success) return NotFound(result.Message);
            return Ok(result.Data);
        }

        [HttpPost("{id}/publish")]
        [Authorize(Roles = Roles.Admin)]
        public IActionResult PublishForm(string id)
        {
            var publishedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
            var result = _formManager.PublishForm(id, publishedBy);
            if (!result.Success) return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}
