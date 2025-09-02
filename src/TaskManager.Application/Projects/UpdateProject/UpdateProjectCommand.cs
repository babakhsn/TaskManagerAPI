using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Projects.UpdateProject;

public sealed record UpdateProjectCommand(Guid OwnerId, Guid ProjectId, string Name)
    : IRequest<ProjectDto?>; // null => not found or not owner
