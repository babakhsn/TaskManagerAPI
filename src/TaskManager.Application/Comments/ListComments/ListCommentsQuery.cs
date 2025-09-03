using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Comments.ListComments;

public sealed record ListCommentsQuery(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId
) : IRequest<IReadOnlyList<CommentDto>>;
