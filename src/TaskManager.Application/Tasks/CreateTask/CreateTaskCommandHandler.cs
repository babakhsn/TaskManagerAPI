using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tasks.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto?>
{
    private readonly IApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(IApplicationDbContext db, IMapper mapper)
    {
        _db = db; _mapper = mapper;
    }

    public async Task<TaskDto?> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        // Enforce ownership: project must belong to the caller
        var project = await _db.Projects
            .Where(p => p.Id == request.ProjectId && p.OwnerId == request.OwnerId)
            .FirstOrDefaultAsync(ct);

        if (project is null) return null;

        var entity = new TaskItem(
            request.ProjectId,
            request.Title,
            request.Description,
            request.DueDateUtc
        );
        entity.UpdateDetails(request.Title, request.Description, request.DueDateUtc, request.Priority);

        _db.TaskItems.Add(entity);
        await _db.SaveChangesAsync(ct);

        return _mapper.Map<TaskDto>(entity);
    }
}
