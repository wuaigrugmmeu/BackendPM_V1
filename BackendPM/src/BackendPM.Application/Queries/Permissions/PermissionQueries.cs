using System;
using System.Collections.Generic;
using BackendPM.Application.DTOs;
using MediatR;

namespace BackendPM.Application.Queries.Permissions;

/// <summary>
/// 获取所有权限查询
/// </summary>
public record GetAllPermissionsQuery() : IRequest<List<PermissionDto>>;

/// <summary>
/// 根据分组获取权限查询
/// </summary>
public record GetPermissionsByGroupQuery(string Group) : IRequest<List<PermissionDto>>;

/// <summary>
/// 根据ID获取权限查询
/// </summary>
public record GetPermissionByIdQuery(Guid Id) : IRequest<PermissionDto>;

/// <summary>
/// 获取角色权限查询
/// </summary>
public record GetRolePermissionsQuery(Guid RoleId) : IRequest<List<PermissionDto>>;

/// <summary>
/// 获取用户权限查询
/// </summary>
public record GetUserPermissionsQuery(Guid UserId) : IRequest<List<PermissionDto>>;