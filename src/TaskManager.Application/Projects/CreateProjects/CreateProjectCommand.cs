using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Projects.CreateProject;

public sealed record CreateProjectCommand(Guid OwnerId, string Name) : IRequest<ProjectDto>;
