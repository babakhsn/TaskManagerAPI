using TaskManager.Domain.Common;

namespace TaskManager.Domain.Entities;

public class Project : EntityBase
{
    public string Name { get; private set; }
    public Guid OwnerId { get; private set; }

    //private readonly List<TaskItem> _tasks = new();
    //public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public List<TaskItem> Tasks { get; private set; } = new();
    public Project(string name, Guid ownerId)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Name = name.Trim();
        OwnerId = ownerId;
    }

    public TaskItem AddTask(string title, string? description = null, DateTime? dueDateUtc = null)
    {
        Guard.AgainstNullOrEmpty(title, nameof(title));
        Guard.AgainstPast(dueDateUtc, nameof(dueDateUtc));

        var task = new TaskItem(this.Id, title, description, dueDateUtc);
        Tasks.Add(task);
        Touch();
        return task;
    }

    public void Rename(string name)
    {
        Guard.AgainstNullOrEmpty(name, nameof(name));
        Name = name.Trim();
        Touch();
    }
}
