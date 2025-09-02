using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Projects.CreateProject;
using TaskManager.Application.Projects.DTOs;
using TaskManager.Application.Projects.ListProjects;
using TaskManager.Application.Projects.UpdateProject;
using TaskManager.Application.Projects.DeleteProject;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> ListMyProjects()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var ownerId))
            return Forbid(); // or BadRequest if you prefer

        var result = await _mediator.Send(new ListProjectsQuery(ownerId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest body)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var ownerId))
            return Forbid();

        var dto = await _mediator.Send(new CreateProjectCommand(ownerId, body.Name));
        // Location header could point to GET /api/projects (or /api/projects/{id} when you add it)
        return CreatedAtAction(nameof(ListMyProjects), new { }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, [FromBody] UpdateProjectRequest body)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var ownerId)) return Forbid();

        var updated = await _mediator.Send(new UpdateProjectCommand(ownerId, id, body.Name));
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var ownerId)) return Forbid();

        var ok = await _mediator.Send(new DeleteProjectCommand(ownerId, id));
        return ok ? NoContent() : NotFound();
    }
}
