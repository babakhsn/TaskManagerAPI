using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Application.Tasks.DTOs;
using TaskManager.Application.Tasks.ListByProject;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    public TasksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> List(Guid projectId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var ownerId)) return Forbid();

        var result = await _mediator.Send(new ListTasksByProjectQuery(ownerId, projectId));
        // If not owner, we return empty list (ownership enforced in handler). You can change to 404 if you prefer.
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(Guid projectId, [FromBody] CreateTaskRequest body)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var ownerId)) return Forbid();

        var created = await _mediator.Send(new CreateTaskCommand(
            ownerId,
            projectId,
            body.Title,
            body.Description,
            body.DueDateUtc,
            body.Priority
        ));

        if (created is null) return NotFound(); // project doesn't exist or not owner

        // You can add a GET /api/tasks/{id} later; for now point to list of project tasks
        return CreatedAtAction(nameof(List), new { projectId }, created);
    }
}
