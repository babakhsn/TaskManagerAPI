using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> b)
    {
        b.ToTable("Tasks");
        b.HasKey(x => x.Id);

        b.Property(x => x.ProjectId).IsRequired();

        b.Property(x => x.ProjectId).IsRequired();
        b.Property(x => x.Title).IsRequired().HasMaxLength(300);
        b.Property(x => x.Description).HasMaxLength(4000);

        b.Property(x => x.Status).HasConversion<int>().HasDefaultValue(Domain.Enums.TaskStatus.Open).IsRequired();
        b.Property(x => x.Priority).HasConversion<int>().IsRequired();

        b.Property(x => x.DueDateUtc);
        b.Property(x => x.AssigneeId);
        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);

        b.HasMany(t => t.Comments)
         .WithOne()
         .HasForeignKey(c => c.TaskId)
         .OnDelete(DeleteBehavior.Cascade);

        // Indexes for common filters
        b.HasIndex(x => new { x.ProjectId, x.Status });
        b.HasIndex(x => new { x.ProjectId, x.AssigneeId });
        b.HasIndex(x => x.DueDateUtc);
    }
}
