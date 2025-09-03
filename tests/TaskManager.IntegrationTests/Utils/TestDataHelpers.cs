using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.IntegrationTests.Utils;

public static class TestDataHelpers
{
    public static async Task AddMemberAsync(Guid projectId, Guid userId)
    {
        // Use the *latest* factory’s service provider; simplest is a new one:
        using var f = new TestWebApplicationFactory();
        using var scope = f.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.ProjectMembers.Add(new ProjectMember(projectId, userId));
        await db.SaveChangesAsync();
    }
}
