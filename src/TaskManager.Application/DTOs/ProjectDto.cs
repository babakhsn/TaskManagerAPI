namespace TaskManager.Application.DTOs;

public record ProjectDto(Guid Id, string Name, Guid OwnerId, DateTime CreatedAtUtc);
