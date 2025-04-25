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

namespace BackendPM.Application.Commands.Permissions;

/// <summary>
/// 创建权限命令处理器
/// </summary>
public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreatePermissionCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public CreatePermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreatePermissionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理创建权限命令
    /// </summary>
    public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        // 检查编码是否已存在
        var existingPermission = await _unitOfWork.Permissions
            .GetSingleAsync(p => p.Code == request.Code);
            
        if (existingPermission != null)
        {
            throw new BusinessRuleViolationException($"编码为 {request.Code} 的权限已存在");
        }

        // 创建权限实体
        var permission = new Permission(
            Guid.NewGuid(),
            request.Name,
            request.Code,
            request.Description,
            request.Group,
            request.ResourcePath,
            request.HttpMethod,
            request.ResourceType,
            request.SortOrder,
            isSystem: false);

        // 保存到数据库
        await _unitOfWork.Permissions.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已创建权限 {PermissionName} (ID: {PermissionId})", permission.Name, permission.Id);

        // 返回DTO
        return _mapper.Map<PermissionDto>(permission);
    }
}