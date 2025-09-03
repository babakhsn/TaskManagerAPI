using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Tasks.ListByProject;

public class ListTasksByProjectQueryHandler
    : IRequestHandler<ListTasksByProjectQuery, IReadOnlyList<TaskDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ListTasksByProjectQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db; _mapper = mapper;
    }

    public async Task<IReadOnlyList<TaskDto>> Handle(ListTasksByProjectQuery request, CancellationToken ct)
    {
        // Ownership enforcement: only list if caller owns the project
        var ownsProject = await _db.Projects
            .AnyAsync(p => p.Id == request.ProjectId && p.OwnerId == request.OwnerId, ct);

        if (!ownsProject) return Array.Empty<TaskDto>();

        return await _db.TaskItems
            .Where(t => t.ProjectId == request.ProjectId)
            .OrderBy(t => t.DueDateUtc ?? DateTime.MaxValue)
            .ThenBy(t => t.CreatedAtUtc)
            .ProjectTo<TaskDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
