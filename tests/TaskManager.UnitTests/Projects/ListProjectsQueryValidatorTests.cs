using FluentAssertions;
using TaskManager.Application.Projects.ListProjects;

namespace TaskManager.UnitTests.Projects;

public class ListProjectsQueryValidatorTests
{
    [Fact]
    public void Should_fail_when_owner_empty()
    {
        var v = new ListProjectsQueryValidator();
        var result = v.Validate(new ListProjectsQuery(Guid.Empty));
        result.IsValid.Should().BeFalse();
    }
}
