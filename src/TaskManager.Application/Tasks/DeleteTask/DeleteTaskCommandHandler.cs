using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;

namespace TaskManager.Application.Tasks.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly IApplicationDbContext _db;
    public DeleteTaskCommandHandler(IApplicationDbContext db) { _db = db; }

    public async Task<bool> Handle(DeleteTaskCommand r, CancellationToken ct)
    {
        var authorized = await _db.Projects
            .AnyAsync(p => p.Id == r.ProjectId && (
                p.OwnerId == r.ActorId ||
                _db.ProjectMembers.Any(m => m.ProjectId == p.Id && m.UserId == r.ActorId)
            ), ct);
        if (!authorized) return false;

        var task = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == r.TaskId && t.ProjectId == r.ProjectId, ct);
        if (task is null) return false;

        _db.TaskItems.Remove(task);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
