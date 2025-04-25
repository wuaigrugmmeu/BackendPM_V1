using System;
using BackendPM.Application.DTOs;
using MediatR;

namespace BackendPM.Application.Commands.Permissions;

/// <summary>
/// 创建权限命令
/// </summary>
public record CreatePermissionCommand(
    string Name,
    string Code,
    string? Description,
    string Group,
    string ResourcePath,
    string? HttpMethod,
    string ResourceType,
    int SortOrder) : IRequest<PermissionDto>;

/// <summary>
/// 更新权限命令
/// </summary>
public record UpdatePermissionCommand(
    Guid Id,
    string Name,
    string? Description,
    string Group,
    string ResourcePath,
    string? HttpMethod,
    int SortOrder) : IRequest<PermissionDto>;

/// <summary>
/// 删除权限命令
/// </summary>
public record DeletePermissionCommand(Guid Id) : IRequest;

/// <summary>
/// 分配权限到角色命令
/// </summary>
public record AssignPermissionToRoleCommand(Guid PermissionId, Guid RoleId) : IRequest;

/// <summary>
/// 从角色移除权限命令
/// </summary>
public record RemovePermissionFromRoleCommand(Guid PermissionId, Guid RoleId) : IRequest;

/// <summary>
/// 批量分配权限到角色命令
/// </summary>
public record BatchAssignPermissionsToRoleCommand(Guid[] PermissionIds, Guid RoleId) : IRequest;