using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> b)
    {
        b.ToTable("Projects");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.OwnerId).IsRequired();
        b.Property(x => x.CreatedAtUtc).IsRequired();
        b.Property(x => x.UpdatedAtUtc);

        // Relationship: Project 1 - many TaskItems
        b.HasMany(p => p.Tasks)
         .WithOne()
         .HasForeignKey(t => t.ProjectId)
         .OnDelete(DeleteBehavior.Cascade);

        //b.Navigation(p => p.Tasks)
        // .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Useful index for listing by owner
        b.HasIndex(x => new { x.OwnerId, x.Name });
    }
}
