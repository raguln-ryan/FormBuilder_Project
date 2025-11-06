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
    public IActionResult CreateFormConfig([FromBody] FormConfigRequestDto dto)
    {
        var adminUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
        var result = _formManager.CreateFormConfig(dto, adminUser);
        if (!result.Success) return BadRequest(result.Message);
        return CreatedAtAction(nameof(GetFormById), new { id = result.Data.FormId }, result.Data);
    }

    
    [HttpPut("FormConfig/{id}")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult UpdateFormConfig(string id, [FromBody] FormConfigRequestDto dto)
    {
        var result = _formManager.UpdateFormConfig(id, dto);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    // -------------------- Form Layout (Single Endpoint - Update Only) --------------------
    /// <summary>
    /// Updates form layout (questions) for an existing form
    /// </summary>
    /// <param name="id">Existing Form ID</param>
    /// <param name="dto">Questions to set for the form</param>
    /// <returns>Updated form layout with questions</returns>
    /// <remarks>
    /// Note: Form must exist (created via FormConfig) before updating layout.
    /// This endpoint only manages questions, not title/description.
    /// </remarks>
    /// <response code="200">Form layout successfully updated</response>
    /// <response code="400">Form not found or form is published</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("Layout/{id}")]
    [Authorize(Roles = Roles.Admin)]
   
    public IActionResult UpdateFormLayout(string id, [FromBody] FormLayoutRequestDto dto)
    {
        var adminUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
        
        // Remove this line - dto no longer has FormId property
        // dto.FormId = id; 
        
        // Pass id directly to the manager instead of dto.FormId
        var result = _formManager.UpdateFormLayout(id, dto, adminUser);
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
        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// Gets all forms with pagination and search support
    /// </summary>
    /// <param name="offset">Number of items to skip (default: 0)</param>
    /// <param name="limit">Number of items to retrieve (default: 10, max: 100)</param>
    /// <param name="search">Search term to filter forms by title, description, or questions</param>
    /// <returns>Paginated list of forms</returns>
    /// <response code="200">Forms retrieved successfully</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet]
    [Authorize]
    public IActionResult GetAllForms(
        [FromQuery] int page = 1, 
        [FromQuery] int size = 10,
        [FromQuery] string search = null)
    {
        var result = _formManager.GetAllForms(User, page, size, search);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetFormById(string id)
    {
        var result = _formManager.GetFormById(id, User);
        if (!result.Success) return NotFound(result.Message);
        return Ok(result.Data);
    }

  
    [HttpPut("{id}/publish")]
    [Authorize(Roles = Roles.Admin)]
    public IActionResult PublishForm(string id)
    {
        var publishedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "Admin";
        var result = _formManager.PublishForm(id, publishedBy);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(new { message = result.Message });
    }
}
}
