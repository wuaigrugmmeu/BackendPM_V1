using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;

namespace BackendPM.Application.Mappings;

/// <summary>
/// 部门相关对象映射配置
/// </summary>
public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        // 实体到基础DTO的映射
        CreateMap<Department, DepartmentBasicDto>();
        
        // 实体到DTO的映射
        CreateMap<Department, DepartmentDto>()
            .ForMember(dest => dest.ParentDepartmentName, 
                opt => opt.MapFrom(src => src.ParentDepartment != null ? src.ParentDepartment.Name : null))
            .ForMember(dest => dest.ChildDepartments, 
                opt => opt.MapFrom(src => src.ChildDepartments));

        // UserDepartment到UserDepartmentDto的映射
        CreateMap<UserDepartment, UserDepartmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Department.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Department.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Department.Code));
    }
}