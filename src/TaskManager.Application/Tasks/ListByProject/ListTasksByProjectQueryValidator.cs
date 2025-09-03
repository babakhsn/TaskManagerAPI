using FluentValidation;

namespace TaskManager.Application.Tasks.ListByProject;

public class ListTasksByProjectQueryValidator : AbstractValidator<ListTasksByProjectQuery>
{
    public ListTasksByProjectQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
    }
}
