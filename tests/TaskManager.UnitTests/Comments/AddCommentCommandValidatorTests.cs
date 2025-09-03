using FluentAssertions;
using TaskManager.Application.Comments.AddComment;

namespace TaskManager.UnitTests.Comments;

public class AddCommentCommandValidatorTests
{
    [Fact]
    public void Should_fail_on_empty_body()
    {
        var v = new AddCommentCommandValidator();
        var r = v.Validate(new AddCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ""));
        r.IsValid.Should().BeFalse();
    }
}
