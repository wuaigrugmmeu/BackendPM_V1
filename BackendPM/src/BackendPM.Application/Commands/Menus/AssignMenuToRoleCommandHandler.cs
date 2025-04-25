using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Menus;

/// <summary>
/// 分配菜单到角色命令处理器
/// </summary>
public class AssignMenuToRoleCommandHandler : IRequestHandler<AssignMenuToRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignMenuToRoleCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AssignMenuToRoleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<AssignMenuToRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理分配菜单到角色命令
    /// </summary>
    public async Task Handle(AssignMenuToRoleCommand request, CancellationToken cancellationToken)
    {
        // 检查菜单是否存在
        var menu = await _unitOfWork.Menus.GetByIdAsync(request.MenuId);
        if (menu == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.MenuId} 的菜单");
        }

        // 检查角色是否存在
        var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.RoleId} 的角色");
        }

        // 检查是否已经分配
        var isAlreadyAssigned = await _unitOfWork.Roles
            .AsQueryable()
            .Where(r => r.Id == request.RoleId)
            .SelectMany(r => r.RoleMenus)
            .AnyAsync(rm => rm.MenuId == request.MenuId, cancellationToken);

        if (isAlreadyAssigned)
        {
            // 如果已经分配，则直接返回
            return;
        }

        // 创建角色菜单关联
        var roleMenu = new RoleMenu(role.Id, menu.Id);
        
        // 添加菜单到角色
        await role.AddMenuAsync(menu, roleMenu);
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "已将菜单 {MenuName} (ID: {MenuId}) 分配给角色 {RoleName} (ID: {RoleId})", 
            menu.Name, menu.Id, role.Name, role.Id);
    }
}