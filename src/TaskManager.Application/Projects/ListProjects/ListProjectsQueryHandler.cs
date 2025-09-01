using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Projects.ListProjects;

public class ListProjectsQueryHandler : IRequestHandler<ListProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public ListProjectsQueryHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProjectDto>> Handle(ListProjectsQuery request, CancellationToken ct)
    {
        return await _db.Projects
            .Where(p => p.OwnerId == request.OwnerId)
            .OrderBy(p => p.Name)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
