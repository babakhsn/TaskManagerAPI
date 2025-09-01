using FluentValidation;

namespace TaskManager.Application.Projects.ListProjects;

public class ListProjectsQueryValidator : AbstractValidator<ListProjectsQuery>
{
    public ListProjectsQueryValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
    }
}
