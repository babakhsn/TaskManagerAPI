using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using static System.Net.WebRequestMethods;

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


//HTTP Request → DTO → Domain Model → Entity(EF Core)
//Entity → Domain Model → DTO → HTTP Response
