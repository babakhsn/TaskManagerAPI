using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions;
using TaskManager.Application.DTOs;
using TaskManager.Application.Mappings;
using TaskManager.Application.Projects.CreateProject;
using TaskManager.Application.Projects.ListProjects;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;
using Microsoft.Extensions.Logging.Abstractions;   // <- add this


namespace TaskManager.UnitTests.Projects;

public class ProjectHandlersTests
{
    private static (IApplicationDbContext db, IMapper mapper) MakeContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique db for each test
            .Options;

        var db = new ApplicationDbContext(options);

        var cfg = new MapperConfiguration(
            c => c.AddProfile<DomainToDtoProfile>(),
            NullLoggerFactory.Instance                // <- required by AutoMapper v15
        );


        var mapper = cfg.CreateMapper();

        return (db, mapper);
    }

    [Fact]
    public async Task CreateProject_Should_Persist_And_Return_Dto()
    {
        var (db, mapper) = MakeContext();
        var handler = new CreateProjectCommandHandler(db, mapper);

        var ownerId = Guid.NewGuid();
        var dto = await handler.Handle(new CreateProjectCommand(ownerId, "My Project"), default);

        dto.Should().NotBeNull();
        dto.Name.Should().Be("My Project");

        var exists = await ((ApplicationDbContext)db).Projects.AnyAsync(p => p.Id == dto.Id);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ListProjects_Should_Filter_By_Owner()
    {
        var (db, mapper) = MakeContext();

        // seed 2 owners
        var a = new Project("A1", Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        var b = new Project("B1", Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
        db.Projects.AddRange(a, b);
        await db.SaveChangesAsync(default);

        var handler = new ListProjectsQueryHandler(db, mapper);

        var listA = await handler.Handle(new ListProjectsQuery(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")), default);
        listA.Should().ContainSingle().Which.Name.Should().Be("A1");

        var listB = await handler.Handle(new ListProjectsQuery(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")), default);
        listB.Should().ContainSingle().Which.Name.Should().Be("B1");
    }
}
