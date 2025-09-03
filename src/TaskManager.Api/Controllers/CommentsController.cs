using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Comments.AddComment;
using TaskManager.Application.Comments.DTOs;
using TaskManager.Application.Comments.ListComments;
using TaskManager.Application.DTOs;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks/{taskId:guid}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> List(Guid projectId, Guid taskId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var actorId)) return Forbid();

        var result = await _mediator.Send(new ListCommentsQuery(actorId, projectId, taskId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(Guid projectId, Guid taskId, [FromBody] CreateCommentRequest body)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var actorId)) return Forbid();

        var dto = await _mediator.Send(new AddCommentCommand(actorId, projectId, taskId, body.Body));
        if (dto is null) return NotFound(); // not owner/member or task not found
        return CreatedAtAction(nameof(List), new { projectId, taskId }, dto);
    }
}
