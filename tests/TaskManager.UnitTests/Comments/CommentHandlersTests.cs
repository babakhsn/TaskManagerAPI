using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Comments.AddComment;
using TaskManager.Application.Comments.ListComments;
using TaskManager.Application.Mappings;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.UnitTests.Comments;

public class CommentHandlersTests
{
    private static (IApplicationDbContext db, IMapper mapper) Make()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new ApplicationDbContext(options);

        var cfg = new AutoMapper.MapperConfiguration(c => c.AddProfile<DomainToDtoProfile>(), NullLoggerFactory.Instance);
        var mapper = cfg.CreateMapper();
        return (db, mapper);
    }

    [Fact]
    public async Task Owner_Can_Add_And_List_Comments()
    {
        var (db, mapper) = Make();
        var owner = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var p = new Project("P", owner);
        db.Projects.Add(p);
        await db.SaveChangesAsync();

        var t = new TaskItem(p.Id, "T", null, null);
        db.TaskItems.Add(t);
        await db.SaveChangesAsync();

        var add = new AddCommentCommandHandler(db, mapper);
        var dto = await add.Handle(new AddCommentCommand(owner, p.Id, t.Id, "hello"), default);
        dto.Should().NotBeNull();

        var list = new ListCommentsQueryHandler(db, mapper);
        var items = await list.Handle(new ListCommentsQuery(owner, p.Id, t.Id), default);
        items.Should().ContainSingle().Which.Body.Should().Be("hello");
    }

    [Fact]
    public async Task NonMember_Cannot_Add()
    {
        var (db, mapper) = Make();
        var owner = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var outsider = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var p = new Project("P", owner);
        db.Projects.Add(p);
        await db.SaveChangesAsync();

        var t = new TaskItem(p.Id, "T", null, null);
        db.TaskItems.Add(t);
        await db.SaveChangesAsync();

        var add = new AddCommentCommandHandler(db, mapper);
        var dto = await add.Handle(new AddCommentCommand(outsider, p.Id, t.Id, "nope"), default);
        dto.Should().BeNull();
    }
}
