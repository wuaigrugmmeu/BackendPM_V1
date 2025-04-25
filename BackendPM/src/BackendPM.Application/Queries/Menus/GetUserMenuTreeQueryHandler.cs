using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Application.Queries.Menus;

/// <summary>
/// 获取用户菜单树形结构查询处理器
/// </summary>
public class GetUserMenuTreeQueryHandler : IRequestHandler<GetUserMenuTreeQuery, List<MenuTreeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetUserMenuTreeQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 处理获取用户菜单树形结构查询
    /// </summary>
    public async Task<List<MenuTreeDto>> Handle(GetUserMenuTreeQuery request, CancellationToken cancellationToken)
    {
        // 检查用户是否存在
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.UserId} 的用户");
        }

        // 获取用户的所有角色
        var roles = await _unitOfWork.Users
            .AsQueryable()
            .Where(u => u.Id == request.UserId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role)
            .ToListAsync(cancellationToken);

        // 获取角色的所有菜单ID
        var menuIds = new HashSet<Guid>();
        foreach (var role in roles)
        {
            var roleMenus = await _unitOfWork.Roles
                .AsQueryable()
                .Where(r => r.Id == role.Id)
                .SelectMany(r => r.RoleMenus)
                .Select(rm => rm.MenuId)
                .ToListAsync(cancellationToken);

            foreach (var menuId in roleMenus)
            {
                menuIds.Add(menuId);
            }
        }

        // 如果用户没有任何菜单权限，则返回空列表
        if (!menuIds.Any())
        {
            return new List<MenuTreeDto>();
        }

        // 获取所有相关菜单
        var allMenus = await _unitOfWork.Menus
            .AsQueryable()
            .Where(m => menuIds.Contains(m.Id) && m.Visible)
            .ToListAsync(cancellationToken);

        // 获取所有可见的根菜单（没有父菜单的菜单）
        var rootMenus = allMenus
            .Where(m => m.ParentMenuId == null)
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Name)
            .ToList();

        // 构建树形结构
        var result = new List<MenuTreeDto>();
        foreach (var menu in rootMenus)
        {
            result.Add(BuildMenuTree(menu, allMenus));
        }

        return result;
    }

    /// <summary>
    /// 构建菜单树形结构
    /// </summary>
    private MenuTreeDto BuildMenuTree(Menu menu, List<Menu> allMenus)
    {
        var dto = _mapper.Map<MenuTreeDto>(menu);
        
        // 获取当前菜单的可见子菜单
        var children = allMenus
            .Where(m => m.ParentMenuId == menu.Id && m.Visible)
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Name)
            .ToList();

        // 递归构建子菜单的树形结构
        foreach (var child in children)
        {
            dto.Children.Add(BuildMenuTree(child, allMenus));
        }

        return dto;
    }
}