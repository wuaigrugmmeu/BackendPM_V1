using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Menus;

/// <summary>
/// 更新菜单命令处理器
/// </summary>
public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateMenuCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UpdateMenuCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateMenuCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理更新菜单命令
    /// </summary>
    public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        // 获取要更新的菜单
        var menu = await _unitOfWork.Menus.GetByIdAsync(request.Id);
        if (menu == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.Id} 的菜单");
        }

        // 检查是否为系统预设菜单
        if (menu.IsSystem)
        {
            throw new BusinessRuleViolationException("不能修改系统预设菜单");
        }

        // 检查父菜单是否存在
        if (request.ParentMenuId.HasValue)
        {
            var parentMenu = await _unitOfWork.Menus.GetByIdAsync(request.ParentMenuId.Value);
            if (parentMenu == null)
            {
                throw new EntityNotFoundException($"找不到ID为 {request.ParentMenuId.Value} 的父菜单");
            }

            // 确保不会创建循环引用
            if (request.ParentMenuId.Value == menu.Id)
            {
                throw new BusinessRuleViolationException("菜单不能将自己设为父菜单");
            }
        }

        // 更新菜单信息
        menu.UpdateInfo(
            request.Name,
            request.Path,
            request.Icon,
            request.Component,
            request.ParentMenuId,
            request.SortOrder,
            request.Visible);

        // 保存到数据库
        _unitOfWork.Menus.Update(menu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已更新菜单 {MenuName} (ID: {MenuId})", menu.Name, menu.Id);

        // 返回DTO
        return _mapper.Map<MenuDto>(menu);
    }
}