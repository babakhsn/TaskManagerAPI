using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Mappings;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Application.Tasks.ListByProject;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Persistence;


namespace TaskManager.UnitTests.Tasks;

public class TaskHandlersTests
{
    private static (IApplicationDbContext db, IMapper mapper) Make()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new ApplicationDbContext(options);

        // ✅ AutoMapper v15: manual config with logger factory (no DI, no AddAutoMapper)
        var cfg = new AutoMapper.MapperConfiguration(
            c => c.AddProfile<DomainToDtoProfile>(),
            NullLoggerFactory.Instance
        );
        var mapper = cfg.CreateMapper();

        return (db, mapper);
    }

    [Fact]
    public async Task CreateTask_Should_Persist_And_Return_Dto_When_Owner()
    {
        var (db, mapper) = Make();

        var ownerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var project = new Project("P1", ownerId);
        db.Projects.Add(project);
        await db.SaveChangesAsync(default);

        var handler = new CreateTaskCommandHandler(db, mapper);
        var dto = await handler.Handle(new CreateTaskCommand(
            ownerId, project.Id, "Task A", "desc", DateTime.UtcNow.Date.AddDays(1), Priority.High
        ), default);

        dto.Should().NotBeNull();
        dto!.Title.Should().Be("Task A");
        dto.Priority.Should().Be(Priority.High);
    }

    [Fact]
    public async Task CreateTask_Should_Return_Null_When_NotOwner()
    {
        var (db, mapper) = Make();

        var ownerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var project = new Project("P1", ownerId);
        db.Projects.Add(project);
        await db.SaveChangesAsync(default);

        var handler = new CreateTaskCommandHandler(db, mapper);
        var dto = await handler.Handle(new CreateTaskCommand(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), project.Id, "X", null, null, Priority.Medium
        ), default);

        dto.Should().BeNull();
    }

    [Fact]
    public async Task ListTasks_Should_Filter_By_Project_And_Owner()
    {
        var (db, mapper) = Make();

        var ownerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var otherOwner = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var p1 = new Project("P1", ownerId);
        var p2 = new Project("P2", otherOwner);

        db.Projects.AddRange(p1, p2);
        await db.SaveChangesAsync(default);

        db.TaskItems.AddRange(
            new TaskItem(p1.Id, "A", null, null),
            new TaskItem(p1.Id, "B", null, null),
            new TaskItem(p2.Id, "C", null, null)
        );
        await db.SaveChangesAsync(default);

        var handler = new ListTasksByProjectQueryHandler(db, mapper);

        var mine = await handler.Handle(new ListTasksByProjectQuery(ownerId, p1.Id), default);
        mine.Should().HaveCount(2).And.OnlyContain(t => t.ProjectId == p1.Id);

        var notOwner = await handler.Handle(new ListTasksByProjectQuery(otherOwner, p1.Id), default);
        notOwner.Should().BeEmpty(); // ownership enforced
    }
}
