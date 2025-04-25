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
/// 删除菜单命令处理器
/// </summary>
public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public DeleteMenuCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteMenuCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理删除菜单命令
    /// </summary>
    public async Task Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        // 获取要删除的菜单
        var menu = await _unitOfWork.Menus.GetByIdAsync(request.Id);
        if (menu == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.Id} 的菜单");
        }

        // 检查是否为系统预设菜单
        if (menu.IsSystem)
        {
            throw new BusinessRuleViolationException("不能删除系统预设菜单");
        }

        // 检查是否有子菜单
        var childMenus = await _unitOfWork.Menus
            .AsQueryable()
            .Where(m => m.ParentMenuId == menu.Id)
            .ToListAsync(cancellationToken);

        if (childMenus.Any())
        {
            throw new BusinessRuleViolationException($"菜单 '{menu.Name}' 存在子菜单，无法删除");
        }

        // 检查是否有角色关联
        var hasRoles = await _unitOfWork.Menus
            .AsQueryable()
            .Where(m => m.Id == menu.Id)
            .SelectMany(m => m.RoleMenus)
            .AnyAsync(cancellationToken);

        if (hasRoles)
        {
            throw new BusinessRuleViolationException($"菜单 '{menu.Name}' 存在角色关联，请先解除关联再删除");
        }

        // 删除菜单
        _unitOfWork.Menus.Delete(menu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已删除菜单 {MenuName} (ID: {MenuId})", menu.Name, menu.Id);
    }
}