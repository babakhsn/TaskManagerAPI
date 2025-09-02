using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Projects.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db; _mapper = mapper;
    }

    public async Task<ProjectDto?> Handle(UpdateProjectCommand request, CancellationToken ct)
    {
        var entity = await _db.Projects
            .Where(p => p.Id == request.ProjectId && p.OwnerId == request.OwnerId) // ownership enforced here
            .FirstOrDefaultAsync(ct);

        if (entity is null) return null; // 404 for not found / not owner (no info leak)

        entity.Rename(request.Name);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<ProjectDto>(entity);
    }
}
