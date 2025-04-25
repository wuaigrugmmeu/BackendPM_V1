using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Menus;

/// <summary>
/// 创建菜单命令处理器
/// </summary>
public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateMenuCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public CreateMenuCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateMenuCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理创建菜单命令
    /// </summary>
    public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        // 检查编码是否已存在
        var existingMenu = await _unitOfWork.Menus
            .GetSingleAsync(m => m.Code == request.Code);
            
        if (existingMenu != null)
        {
            throw new BusinessRuleViolationException($"编码为 {request.Code} 的菜单已存在");
        }

        // 检查父菜单是否存在
        Menu? parentMenu = null;
        if (request.ParentMenuId.HasValue)
        {
            parentMenu = await _unitOfWork.Menus.GetByIdAsync(request.ParentMenuId.Value);
            if (parentMenu == null)
            {
                throw new EntityNotFoundException($"找不到ID为 {request.ParentMenuId.Value} 的父菜单");
            }
        }

        // 创建菜单实体
        var menu = new Menu(
            Guid.NewGuid(),
            request.Name,
            request.Code,
            request.Path,
            request.Icon,
            request.Component,
            request.ParentMenuId,
            request.SortOrder,
            request.Visible,
            isSystem: false);

        // 保存到数据库
        await _unitOfWork.Menus.AddAsync(menu);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已创建菜单 {MenuName} (ID: {MenuId})", menu.Name, menu.Id);

        // 返回DTO
        return _mapper.Map<MenuDto>(menu);
    }
}