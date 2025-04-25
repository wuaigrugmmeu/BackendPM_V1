using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Permissions;

/// <summary>
/// 更新权限命令处理器
/// </summary>
public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdatePermissionCommandHandler> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UpdatePermissionCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdatePermissionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 处理更新权限命令
    /// </summary>
    public async Task<PermissionDto> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        // 获取要更新的权限
        var permission = await _unitOfWork.Permissions.GetByIdAsync(request.Id);
        if (permission == null)
        {
            throw new EntityNotFoundException($"找不到ID为 {request.Id} 的权限");
        }

        // 检查是否为系统预设权限
        if (permission.IsSystem)
        {
            throw new BusinessRuleViolationException("不能修改系统预设权限");
        }

        // 更新权限信息
        permission.UpdateInfo(
            request.Name,
            request.Description,
            request.Group,
            request.ResourcePath,
            request.HttpMethod,
            request.SortOrder);

        // 保存到数据库
        _unitOfWork.Permissions.Update(permission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("已更新权限 {PermissionName} (ID: {PermissionId})", permission.Name, permission.Id);

        // 返回DTO
        return _mapper.Map<PermissionDto>(permission);
    }
}