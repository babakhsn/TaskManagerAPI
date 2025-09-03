using MediatR;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    Guid OwnerId,
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDateUtc,
    Priority Priority
) : IRequest<TaskDto?>; // null => project not found or not owner
