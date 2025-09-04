using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Identity;

namespace TaskManager.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // safety: apply pending migrations if any (when called outside Program’s migrate hook)
        await db.Database.MigrateAsync(ct);

        const string email = "demo@task.local";
        const string password = "Passw0rd!";

        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var res = await users.CreateAsync(user, password);
            if (!res.Succeeded)
                throw new InvalidOperationException("Failed to create demo user: " + string.Join(",", res.Errors.Select(e => e.Description)));
        }

        // Already seeded?
        if (await db.Projects.AnyAsync(p => p.OwnerId == Guid.Parse(user.Id), ct))
            return;

        var ownerId = Guid.Parse(user.Id); // Identity uses GUID string by default

        var demoProject = new Project("Demo Project", ownerId);
        db.Projects.Add(demoProject);
        await db.SaveChangesAsync(ct);

        var task = new TaskItem(demoProject.Id, "Your first task", "Edit me ✨", DateTime.UtcNow.Date.AddDays(3));
        task.UpdateDetails(task.Title, task.Description, task.DueDateUtc, Priority.Medium);
        task.SetStatus(Domain.Enums.TaskStatus.InProgress);
        db.TaskItems.Add(task);

        db.Comments.Add(new Comment(task.Id, ownerId, "Welcome! This comment was seeded."));

        // (Optional) add a member:
        // db.ProjectMembers.Add(new ProjectMember(demoProject.Id, someOtherUserId));

        await db.SaveChangesAsync(ct);
    }
}
