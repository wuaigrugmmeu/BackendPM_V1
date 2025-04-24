using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;

namespace BackendPM.Application.Mappings;

/// <summary>
/// 菜单相关对象映射配置
/// </summary>
public class MenuProfile : Profile
{
    public MenuProfile()
    {
        // 实体到基础DTO的映射
        CreateMap<Menu, MenuBasicDto>();
        
        // 实体到DTO的映射
        CreateMap<Menu, MenuDto>()
            .ForMember(dest => dest.ParentMenuName, 
                opt => opt.MapFrom(src => src.ParentMenu != null ? src.ParentMenu.Name : null))
            .ForMember(dest => dest.ChildMenus, 
                opt => opt.MapFrom(src => src.ChildMenus));

        // RoleMenu到RoleMenuDto的映射
        CreateMap<RoleMenu, RoleMenuDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Menu.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Menu.Name))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Menu.Code))
            .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Menu.Path))
            .ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Menu.Icon));
    }
}