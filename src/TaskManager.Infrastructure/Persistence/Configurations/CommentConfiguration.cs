using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> b)
    {
        b.ToTable("Comments");
        b.HasKey(x => x.Id);

        b.Property(x => x.TaskId).IsRequired();
        b.Property(x => x.AuthorId).IsRequired();

        b.Property(x => x.Body).IsRequired().HasMaxLength(4000);
        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);

        //// Relationship: TaskItem 1 - many Comments
        //b.HasOne<TaskItem>()
        // .WithMany("_comments")
        // .HasForeignKey(x => x.TaskId)
        // .OnDelete(DeleteBehavior.Cascade);

        // Useful indexes
        b.HasIndex(x => x.TaskId);
        b.HasIndex(x => x.AuthorId);
    }
}
