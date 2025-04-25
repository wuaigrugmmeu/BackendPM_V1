using System;
using System.Collections.Generic;
using BackendPM.Application.DTOs;
using MediatR;

namespace BackendPM.Application.Queries.Menus;

/// <summary>
/// 获取所有菜单查询
/// </summary>
public record GetAllMenusQuery() : IRequest<List<MenuDto>>;

/// <summary>
/// 获取菜单树形结构查询
/// </summary>
public record GetMenuTreeQuery() : IRequest<List<MenuTreeDto>>;

/// <summary>
/// 根据ID获取菜单查询
/// </summary>
public record GetMenuByIdQuery(Guid Id) : IRequest<MenuDto>;

/// <summary>
/// 获取角色菜单查询
/// </summary>
public record GetRoleMenusQuery(Guid RoleId) : IRequest<List<MenuDto>>;

/// <summary>
/// 获取用户菜单树形结构查询
/// </summary>
public record GetUserMenuTreeQuery(Guid UserId) : IRequest<List<MenuTreeDto>>;