using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;

namespace TaskManager.Application.Projects.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IApplicationDbContext _db;

    public DeleteProjectCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken ct)
    {
        var entity = await _db.Projects
            .Where(p => p.Id == request.ProjectId && p.OwnerId == request.OwnerId) // ownership enforced here
            .FirstOrDefaultAsync(ct);

        if (entity is null) return false;

        _db.Projects.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
