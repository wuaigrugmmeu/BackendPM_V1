using System;
using BackendPM.Application.DTOs;
using MediatR;

namespace BackendPM.Application.Commands.Menus;

/// <summary>
/// 创建菜单命令
/// </summary>
public record CreateMenuCommand(
    string Name,
    string Code,
    string? Path,
    string? Icon,
    string? Component,
    Guid? ParentMenuId,
    int SortOrder,
    bool Visible) : IRequest<MenuDto>;

/// <summary>
/// 更新菜单命令
/// </summary>
public record UpdateMenuCommand(
    Guid Id,
    string Name,
    string? Path,
    string? Icon,
    string? Component,
    Guid? ParentMenuId,
    int SortOrder,
    bool Visible) : IRequest<MenuDto>;

/// <summary>
/// 删除菜单命令
/// </summary>
public record DeleteMenuCommand(Guid Id) : IRequest;

/// <summary>
/// 分配菜单到角色命令
/// </summary>
public record AssignMenuToRoleCommand(Guid MenuId, Guid RoleId) : IRequest;

/// <summary>
/// 从角色移除菜单命令
/// </summary>
public record RemoveMenuFromRoleCommand(Guid MenuId, Guid RoleId) : IRequest;