using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Constants;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Roles;

/// <summary>
/// 更新角色命令
/// </summary>
public class UpdateRoleCommand : BaseCommand<RoleDto>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; }
    
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; }
    
    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; }
    
    public UpdateRoleCommand(Guid roleId, string name, string? description, List<Guid>? permissionIds = null)
    {
        RoleId = roleId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        PermissionIds = permissionIds ?? new List<Guid>();
    }
}

/// <summary>
/// 更新角色命令处理器
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRoleCommandHandler> _logger;
    
    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<RoleDto> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        // 获取角色实体
        var role = await _unitOfWork.Roles.GetByIdWithPermissionsAsync(command.RoleId)
            ?? throw new EntityNotFoundException(ErrorMessages.EntityNames.RoleType, command.RoleId);
            
        // 检查是否为系统角色
        if (role.IsSystem)
        {
            throw new BusinessRuleViolationException(ErrorMessages.Role.SystemRoleModificationForbidden);
        }
        
        // 更新角色信息
        role.Update(command.Name, command.Description);
        
        // 清除现有权限并添加新权限
        var currentPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
        
        // 需要移除的权限
        foreach (var permissionId in currentPermissionIds.Except(command.PermissionIds))
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
            if (permission != null)
            {
                role.RemovePermission(permission);
            }
        }
        
        // 需要添加的权限
        foreach (var permissionId in command.PermissionIds.Except(currentPermissionIds))
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
            if (permission != null)
            {
                role.AddPermission(permission);
            }
        }
        
        // 保存更改
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("角色 {RoleName} (ID: {RoleId}) 已更新", role.Name, role.Id);
        
        // 返回更新后的角色DTO
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Description = role.Description,
            IsSystemRole = role.IsSystem,
            CreatedAt = role.CreatedAt,
            LastModifiedAt = role.LastModifiedAt,
            Permissions = role.RolePermissions
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Code = rp.Permission.Code,
                    Group = rp.Permission.Group,
                    Description = rp.Permission.Description
                })
                .ToList()
        };
    }
}