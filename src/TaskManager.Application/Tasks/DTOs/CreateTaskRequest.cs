using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.DTOs;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    DateTime? DueDateUtc,
    Priority Priority = Priority.Medium
);
