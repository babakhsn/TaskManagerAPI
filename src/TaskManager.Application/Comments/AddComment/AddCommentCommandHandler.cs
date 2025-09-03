using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Comments.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public AddCommentCommandHandler(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<CommentDto?> Handle(AddCommentCommand r, CancellationToken ct)
    {
        // authorize: owner or member of the project
        var authorized = await _db.Projects
            .AnyAsync(p => p.Id == r.ProjectId &&
                           (p.OwnerId == r.ActorId ||
                            _db.ProjectMembers.Any(m => m.ProjectId == r.ProjectId && m.UserId == r.ActorId)), ct);
        if (!authorized) return null;

        // ensure task exists in project
        var task = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == r.TaskId && t.ProjectId == r.ProjectId, ct);
        if (task is null) return null;

        var entity = new Comment(r.TaskId, r.ActorId, r.Body);
        _db.Comments.Add(entity);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<CommentDto>(entity);
    }
}
