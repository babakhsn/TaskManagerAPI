using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Mappings;

public class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<Project, ProjectDto>();
        CreateMap<TaskItem, TaskDto>();
        CreateMap<Comment, CommentDto>();
    }
}
