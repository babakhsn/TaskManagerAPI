using System.Xml.Linq;
using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem : EntityBase
{
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.Open;
    public Priority Priority { get; private set; } = Priority.Medium;
    public DateTime? DueDateUtc { get; private set; }
    public Guid? AssigneeId { get; private set; }

    public List<Comment> Comments { get; private set; } = new();
    //private readonly List<Comment> _comments = new();
    //public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    // ctor used by Project.AddTask
    public TaskItem(Guid projectId, string title, string? description = null, DateTime? dueDateUtc = null)
    {
        if (projectId == Guid.Empty) throw new ArgumentException("ProjectId required.", nameof(projectId));
        Guard.AgainstNullOrEmpty(title, nameof(title));
        Guard.AgainstPast(dueDateUtc, nameof(dueDateUtc));

        ProjectId = projectId;
        Title = title.Trim();
        Description = description?.Trim();
        DueDateUtc = dueDateUtc;
    }

    public void UpdateDetails(string title, string? description, DateTime? dueDateUtc, Priority priority)
    {
        Guard.AgainstNullOrEmpty(title, nameof(title));
        Guard.AgainstPast(dueDateUtc, nameof(dueDateUtc));

        Title = title.Trim();
        Description = description?.Trim();
        DueDateUtc = dueDateUtc;
        Priority = priority;
        Touch();
    }

    public void SetStatus(Enums.TaskStatus status) { Status = status; Touch(); }
    public void AssignTo(Guid? userId) { AssigneeId = userId; Touch(); }

    public Comment AddComment(Guid authorId, string body)
    {
        Guard.AgainstNullOrEmpty(body, nameof(body));
        var c = new Comment(this.Id, authorId, body);
        Comments.Add(c);
        Touch();
        return c;
    }
}
