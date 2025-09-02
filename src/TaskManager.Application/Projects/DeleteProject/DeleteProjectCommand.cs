using MediatR;

namespace TaskManager.Application.Projects.DeleteProject;

public sealed record DeleteProjectCommand(Guid OwnerId, Guid ProjectId) : IRequest<bool>; // false => not found / not owner
