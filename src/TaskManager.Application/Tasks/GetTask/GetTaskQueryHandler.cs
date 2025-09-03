using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Tasks.GetTask;

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, TaskDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public GetTaskQueryHandler(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<TaskDto?> Handle(GetTaskQuery request, CancellationToken ct)
    {
        var authorized = await _db.Projects
            .AnyAsync(p => p.Id == request.ProjectId && (
                p.OwnerId == request.OwnerOrMemberId ||
                _db.ProjectMembers.Any(m => m.ProjectId == p.Id && m.UserId == request.OwnerOrMemberId)
            ), ct);

        if (!authorized) return null;

        var task = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId, ct);
        return task is null ? null : _mapper.Map<TaskDto>(task);
    }
}
