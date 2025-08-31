namespace TaskManager.Application.DTOs;

public record CommentDto(Guid Id, Guid TaskId, Guid AuthorId, string Body, DateTime CreatedAtUtc);
