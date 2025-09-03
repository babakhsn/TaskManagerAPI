using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Tasks.GetTask;

public sealed record GetTaskQuery(Guid OwnerOrMemberId, Guid ProjectId, Guid TaskId) : IRequest<TaskDto?>;
