using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Projects.ListProjects;

public sealed record ListProjectsQuery(Guid OwnerId) : IRequest<IReadOnlyList<ProjectDto>>;
