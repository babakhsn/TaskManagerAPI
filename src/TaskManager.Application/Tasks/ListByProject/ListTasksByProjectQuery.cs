using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Tasks.ListByProject;

public sealed record ListTasksByProjectQuery(Guid OwnerId, Guid ProjectId)
    : IRequest<IReadOnlyList<TaskDto>>;
