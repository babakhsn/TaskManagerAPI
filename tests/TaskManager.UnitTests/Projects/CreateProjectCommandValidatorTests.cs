using FluentAssertions;
using TaskManager.Application.Projects.CreateProject;

namespace TaskManager.UnitTests.Projects;

public class CreateProjectCommandValidatorTests
{
    [Fact]
    public void Should_fail_when_name_empty()
    {
        var v = new CreateProjectCommandValidator();
        var result = v.Validate(new CreateProjectCommand(Guid.NewGuid(), ""));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_pass_when_valid()
    {
        var v = new CreateProjectCommandValidator();
        var result = v.Validate(new CreateProjectCommand(Guid.NewGuid(), "Demo"));
        result.IsValid.Should().BeTrue();
    }
}
