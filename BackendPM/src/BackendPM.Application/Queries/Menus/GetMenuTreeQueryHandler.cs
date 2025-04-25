using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Queries.Menus;

/// <summary>
/// 获取菜单树形结构查询处理器
/// </summary>
public class GetMenuTreeQueryHandler : IRequestHandler<GetMenuTreeQuery, List<MenuTreeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMenuTreeQueryHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetMenuTreeQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetMenuTreeQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理获取菜单树形结构查询
    /// </summary>
    public async Task<List<MenuTreeDto>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 获取所有菜单
            var menus = await _unitOfWork.Menus.FindAsync(m => true);

            // 获取根菜单（没有父菜单的菜单）
            var rootMenus = menus
                .Where(m => m.ParentMenuId == null)
                .OrderBy(m => m.SortOrder)
                .ThenBy(m => m.Name)
                .ToList();

            // 构建树形结构
            var result = new List<MenuTreeDto>();
            foreach (var menu in rootMenus)
            {
                result.Add(BuildMenuTree(menu, menus));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取菜单树形结构时发生错误");
            throw;
        }
    }

    /// <summary>
    /// 构建菜单树形结构
    /// </summary>
    private MenuTreeDto BuildMenuTree(Menu menu, List<Menu> allMenus)
    {
        var dto = _mapper.Map<MenuTreeDto>(menu);
        
        // 获取当前菜单的子菜单
        var children = allMenus
            .Where(m => m.ParentMenuId == menu.Id)
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