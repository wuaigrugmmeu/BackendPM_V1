using AutoMapper;
using BackendPM.Application.Commands.Users;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;

namespace BackendPM.Application.Mappings;

/// <summary>
/// 用户相关对象映射配置
/// </summary>
public class UserProfile : Profile
{
    public UserProfile()
    {
        // 实体到DTO的映射
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles,
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)));

        // DTO到命令的映射
        CreateMap<CreateUserDto, CreateUserCommand>();
        CreateMap<UpdateUserDto, UpdateUserCommand>();
    }
}