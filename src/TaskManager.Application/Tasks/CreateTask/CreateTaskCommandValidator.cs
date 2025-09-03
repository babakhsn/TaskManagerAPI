using FluentValidation;

namespace TaskManager.Application.Tasks.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).MaximumLength(4000);

        // Business rule: due date cannot be in the past (UTC date-based)
        RuleFor(x => x.DueDateUtc)
            .Must(d => d == null || d.Value.Date >= DateTime.UtcNow.Date)
            .WithMessage("DueDateUtc cannot be in the past.");
    }
}
