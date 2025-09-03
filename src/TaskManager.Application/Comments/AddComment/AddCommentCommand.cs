using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Comments.AddComment;

public sealed record AddCommentCommand(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId,
    string Body
) : IRequest<CommentDto?>; // null => not found / not authorized
