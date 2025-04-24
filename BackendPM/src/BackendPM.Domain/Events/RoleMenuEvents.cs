using BackendPM.Domain.Entities;
using System.Collections.Generic;

namespace BackendPM.Domain.Events;

/// <summary>
/// 角色菜单添加事件
/// </summary>
public class RoleMenuAddedEvent(Role role, Menu menu) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;

    /// <summary>
    /// 菜单对象
    /// </summary>
    public Menu Menu { get; } = menu;
}

/// <summary>
/// 角色菜单移除事件
/// </summary>
public class RoleMenuRemovedEvent(Role role, Menu menu) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;

    /// <summary>
    /// 菜单对象
    /// </summary>
    public Menu Menu { get; } = menu;
}

/// <summary>
/// 角色菜单批量变更事件
/// </summary>
public class RoleMenusBulkChangedEvent(Role role, IEnumerable<Menu> addedMenus, IEnumerable<Menu> removedMenus) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;

    /// <summary>
    /// 添加的菜单列表
    /// </summary>
    public IEnumerable<Menu> AddedMenus { get; } = addedMenus;

    /// <summary>
    /// 移除的菜单列表
    /// </summary>
    public IEnumerable<Menu> RemovedMenus { get; } = removedMenus;
}