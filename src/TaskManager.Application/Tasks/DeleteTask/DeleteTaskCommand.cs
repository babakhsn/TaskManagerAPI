using MediatR;

namespace TaskManager.Application.Tasks.DeleteTask;

public sealed record DeleteTaskCommand(Guid ActorId, Guid ProjectId, Guid TaskId) : IRequest<bool>; // false => not found/authorized
