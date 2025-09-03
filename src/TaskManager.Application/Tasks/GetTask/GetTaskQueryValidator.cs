using FluentValidation;

namespace TaskManager.Application.Tasks.GetTask;

public class GetTaskQueryValidator : AbstractValidator<GetTaskQuery>
{
    public GetTaskQueryValidator()
    {
        RuleFor(x => x.OwnerOrMemberId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();
    }
}
