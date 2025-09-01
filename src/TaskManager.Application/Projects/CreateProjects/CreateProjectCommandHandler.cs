using AutoMapper;
using MediatR;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Projects.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var entity = new Project(request.Name, request.OwnerId);
        _db.Projects.Add(entity);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ProjectDto>(entity);
    }
}
