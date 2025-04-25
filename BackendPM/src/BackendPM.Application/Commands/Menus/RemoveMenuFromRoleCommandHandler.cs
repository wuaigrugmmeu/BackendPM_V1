using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Menus;

/// <summary>
/// 从角色移除菜单命令处理器
/// </summary>
public class RemoveMenuFromRoleCommandHandler : IRequestHandler<RemoveMenuFromRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveMenuFromRoleCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RemoveMenuFromRoleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<RemoveMenuFromRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理从角色移除菜单命令
    /// </summary>
    public async Task Handle(RemoveMenuFromRoleCommand request, CancellationToken cancellationToken)
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

        // 检查角色是否关联了该菜单
        var roleMenu = await _unitOfWork.Roles
            .AsQueryable()
            .Where(r => r.Id == request.RoleId)
            .SelectMany(r => r.RoleMenus)
            .FirstOrDefaultAsync(rm => rm.MenuId == request.MenuId, cancellationToken);

        if (roleMenu == null)
        {
            // 如果没有关联，则直接返回
            return;
        }

        // 从角色移除菜单
        role.RemoveMenu(menu.Id);
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "已从角色 {RoleName} (ID: {RoleId}) 移除菜单 {MenuName} (ID: {MenuId})", 
            role.Name, role.Id, menu.Name, menu.Id);
    }
}