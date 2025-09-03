using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;
    public UpdateTaskCommandHandler(IApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<TaskDto?> Handle(UpdateTaskCommand r, CancellationToken ct)
    {
        // authorize: owner or member of the project
        var authorized = await _db.Projects
            .AnyAsync(p => p.Id == r.ProjectId && (
                p.OwnerId == r.ActorId ||
                _db.ProjectMembers.Any(m => m.ProjectId == p.Id && m.UserId == r.ActorId)
            ), ct);
        if (!authorized) return null;

        var task = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == r.TaskId && t.ProjectId == r.ProjectId, ct);
        if (task is null) return null;

        if (r.Title is not null) task.UpdateDetails(r.Title, r.Description ?? task.Description, r.DueDateUtc ?? task.DueDateUtc, r.Priority ?? task.Priority);
        else
        {
            // Title unchanged → still allow other fields to change
            var title = task.Title;
            var desc = r.Description ?? task.Description;
            var due = r.DueDateUtc ?? task.DueDateUtc;
            var pri = r.Priority ?? task.Priority;
            task.UpdateDetails(title, desc, due, pri);
        }

        if (r.Status is not null) task.SetStatus(r.Status.Value);
        if (r.AssigneeId is not null) task.AssignTo(r.AssigneeId);

        await _db.SaveChangesAsync(ct);
        return _mapper.Map<TaskDto>(task);
    }
}
