namespace TaskManager.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }

    private ProjectMember() { } // EF
    public ProjectMember(Guid projectId, Guid userId)
    {
        if (projectId == Guid.Empty) throw new ArgumentException(nameof(projectId));
        if (userId == Guid.Empty) throw new ArgumentException(nameof(userId));
        ProjectId = projectId;
        UserId = userId;
    }
}
