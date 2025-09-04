using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.IntegrationTests.Utils;

public static class TestDataSeed
{
    public static async Task<Project> SeedProjectWithTaskAsync(IServiceProvider sp, Guid ownerId)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var p = new Project("Seeded", ownerId);
        db.Projects.Add(p);
        await db.SaveChangesAsync();

        var t = new TaskItem(p.Id, "Seeded Task", null, null);
        db.TaskItems.Add(t);
        await db.SaveChangesAsync();

        return p;
    }
}
