using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.DTOs;

public sealed record UpdateTaskRequest(
    string? Title,
    string? Description,
    DateTime? DueDateUtc,
    Priority? Priority,
    Domain.Enums.TaskStatus? Status,
    Guid? AssigneeId
);
