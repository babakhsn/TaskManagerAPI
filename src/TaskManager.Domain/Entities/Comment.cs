using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Comment : EntityBase
{
    public Guid TaskId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Body { get; private set; }

    public Comment(Guid taskId, Guid authorId, string body)
    {
        if (taskId == Guid.Empty) throw new ArgumentException("TaskId required.", nameof(taskId));
        if (authorId == Guid.Empty) throw new ArgumentException("AuthorId required.", nameof(authorId));
        Guard.AgainstNullOrEmpty(body, nameof(body));

        TaskId = taskId;
        AuthorId = authorId;
        Body = body.Trim();
    }
}
