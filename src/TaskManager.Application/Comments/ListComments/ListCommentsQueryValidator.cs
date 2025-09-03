using FluentValidation;

namespace TaskManager.Application.Comments.ListComments;

public class ListCommentsQueryValidator : AbstractValidator<ListCommentsQuery>
{
    public ListCommentsQueryValidator()
    {
        RuleFor(x => x.ActorId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();
    }
}
