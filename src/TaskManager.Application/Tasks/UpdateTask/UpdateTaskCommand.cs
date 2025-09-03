using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid ActorId,
    Guid ProjectId,
    Guid TaskId,
    string? Title,
    string? Description,
    DateTime? DueDateUtc,
    Priority? Priority,
    Domain.Enums.TaskStatus? Status,
    Guid? AssigneeId
) : IRequest<TaskDto?>; // null => not found or not authorized
