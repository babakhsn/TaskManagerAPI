using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions;

public interface IApplicationDbContext
{
    //DbSet<User> Users { get; }
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> TaskItems { get; }
    DbSet<Comment> Comments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
