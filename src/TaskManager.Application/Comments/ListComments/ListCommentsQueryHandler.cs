using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Comments.ListComments;

public class ListCommentsQueryHandler : IRequestHandler<ListCommentsQuery, IReadOnlyList<CommentDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public ListCommentsQueryHandler(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<IReadOnlyList<CommentDto>> Handle(ListCommentsQuery r, CancellationToken ct)
    {
        var authorized = await _db.Projects
            .AnyAsync(p => p.Id == r.ProjectId &&
                           (p.OwnerId == r.ActorId ||
                            _db.ProjectMembers.Any(m => m.ProjectId == r.ProjectId && m.UserId == r.ActorId)), ct);
        if (!authorized) return Array.Empty<CommentDto>();

        // ensure task exists
        var taskExists = await _db.TaskItems.AnyAsync(t => t.Id == r.TaskId && t.ProjectId == r.ProjectId, ct);
        if (!taskExists) return Array.Empty<CommentDto>();

        return await _db.Comments
            .Where(c => c.TaskId == r.TaskId)
            .OrderBy(c => c.CreatedAtUtc)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}
