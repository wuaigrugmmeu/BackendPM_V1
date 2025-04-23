using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackendPM.Application.DTOs;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Application.Commands.Roles;

/// <summary>
/// 创建角色命令
/// </summary>
public class CreateRoleCommand : BaseCommand<RoleDto>
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; }
    
    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; }
    
    /// <summary>
    /// 权限ID列表
    /// </summary>
    public List<Guid> PermissionIds { get; }
    
    public CreateRoleCommand(string name, string code, string? description, List<Guid>? permissionIds = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Description = description;
        PermissionIds = permissionIds ?? new List<Guid>();
    }
}

/// <summary>
/// 创建角色命令处理器
/// </summary>
public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRoleCommandHandler> _logger;
    
    public CreateRoleCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateRoleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<RoleDto> Handle(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        // 检查角色编码是否已存在
        if (await _unitOfWork.Roles.ExistsAsync(r => r.Code == command.Code))
        {
            throw new InvalidOperationException($"角色编码 '{command.Code}' 已存在");
        }
        
        // 创建新角色
        var role = new Role(command.Name, command.Code, command.Description, false);
        
        // 添加权限
        if (command.PermissionIds.Count > 0)
        {
            foreach (var permissionId in command.PermissionIds)
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission != null)
                {
                    role.AddPermission(permission);
                }
            }
        }
        
        // 保存角色
        await _unitOfWork.Roles.AddAsync(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("创建了新角色 {RoleName} (ID: {RoleId})", role.Name, role.Id);
        
        // 返回角色DTO
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