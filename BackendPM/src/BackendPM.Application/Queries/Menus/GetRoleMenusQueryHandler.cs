using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Application.Queries.Menus;

/// <summary>
/// 获取角色菜单查询处理器
/// </summary>
public class GetRoleMenusQueryHandler : IRequestHandler<GetRoleMenusQuery, List<MenuDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    /// <summary>
    /// 构造函数
    /// </summary>
    public GetRoleMenusQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// 处理获取角色菜单查询
    /// </summary>
    public async Task<List<MenuDto>> Handle(GetRoleMenusQuery request, CancellationToken cancellationToken)
    {
        // 检查角色是否存在
        var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.RoleId} 的角色");
        }

        // 获取角色的菜单
        var menus = await _unitOfWork.Roles
            .AsQueryable()
            .Where(r => r.Id == request.RoleId)
            .SelectMany(r => r.RoleMenus)
            .Include(rm => rm.Menu)
            .ThenInclude(m => m.ParentMenu)
            .Select(rm => rm.Menu)
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MenuDto>>(menus);
    }
}