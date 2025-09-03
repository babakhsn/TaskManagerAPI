using FluentAssertions;
using TaskManager.Application.Tasks.ListByProject;

namespace TaskManager.UnitTests.Tasks;

public class ListTasksByProjectQueryValidatorTests
{
    [Fact]
    public void Should_fail_when_ids_empty()
    {
        var v = new ListTasksByProjectQueryValidator();
        var r = v.Validate(new ListTasksByProjectQuery(Guid.Empty, Guid.Empty));
        r.IsValid.Should().BeFalse();
    }
}
