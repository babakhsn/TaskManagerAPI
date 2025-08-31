using TaskManager.Domain.Enums;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.DTOs;

public record TaskDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskStatus Status,
    Priority Priority,
    DateTime? DueDateUtc,
    Guid? AssigneeId,
    DateTime CreatedAtUtc
);
