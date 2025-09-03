using FluentValidation;

namespace TaskManager.Application.Tasks.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.ActorId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();

        // at least one field to update
        RuleFor(x => new { x.Title, x.Description, x.DueDateUtc, x.Priority, x.Status, x.AssigneeId })
            .Must(o => o.Title != null || o.Description != null || o.DueDateUtc != null || o.Priority != null || o.Status != null || o.AssigneeId != null)
            .WithMessage("Provide at least one field to update.");

        // due date rule (no past)
        RuleFor(x => x.DueDateUtc)
            .Must(d => d == null || d.Value.Date >= DateTime.UtcNow.Date)
            .WithMessage("DueDateUtc cannot be in the past.");
    }
}
