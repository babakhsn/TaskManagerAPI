using FluentAssertions;
using TaskManager.Application.Tasks.CreateTask;
using TaskManager.Domain.Enums;

namespace TaskManager.UnitTests.Tasks;

public class CreateTaskCommandValidatorTests
{
    [Fact]
    public void Should_fail_when_title_empty()
    {
        var v = new CreateTaskCommandValidator();
        var r = v.Validate(new CreateTaskCommand(Guid.NewGuid(), Guid.NewGuid(), "", null, null, Priority.Medium));
        r.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_fail_when_due_in_past()
    {
        var v = new CreateTaskCommandValidator();
        var r = v.Validate(new CreateTaskCommand(Guid.NewGuid(), Guid.NewGuid(), "X", null, DateTime.UtcNow.Date.AddDays(-1), Priority.Medium));
        r.IsValid.Should().BeFalse();
    }
}
