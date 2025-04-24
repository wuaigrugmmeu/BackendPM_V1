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
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)))
            .ForMember(dest => dest.UserRoles, 
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => new UserRoleDto
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.Name,
                    Code = ur.Role.Code,
                    IsSystemRole = ur.Role.IsSystem
                })))
            .ForMember(dest => dest.UserDepartments, 
                opt => opt.MapFrom(src => src.UserDepartments.Select(ud => new UserDepartmentDto
                {
                    Id = ud.Department.Id,
                    Name = ud.Department.Name,
                    Code = ud.Department.Code,
                    IsPrimary = ud.IsPrimary
                })))
            .ForMember(dest => dest.PrimaryDepartment, 
                opt => opt.MapFrom(src => src.UserDepartments
                    .Where(ud => ud.IsPrimary)
                    .Select(ud => new UserDepartmentDto
                    {
                        Id = ud.Department.Id,
                        Name = ud.Department.Name,
                        Code = ud.Department.Code,
                        IsPrimary = true
                    })
                    .FirstOrDefault()));

        // 角色实体到基础角色DTO的映射
        CreateMap<Role, RoleBasicDto>();
        
        // 实体到角色DTO的映射
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.IsSystemRole, 
                opt => opt.MapFrom(src => src.IsSystem))
            .ForMember(dest => dest.ParentRoleName, 
                opt => opt.MapFrom(src => src.ParentRole != null ? src.ParentRole.Name : null))
            .ForMember(dest => dest.ChildRoles, 
                opt => opt.MapFrom(src => src.ChildRoles));

        // DTO到命令的映射
        CreateMap<CreateUserDto, CreateUserCommand>();
        CreateMap<UpdateUserDto, UpdateUserCommand>();
    }
}