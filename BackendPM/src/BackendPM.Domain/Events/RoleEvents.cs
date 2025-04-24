using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Events;

/// <summary>
/// 角色创建事件
/// </summary>
public class RoleCreatedEvent(Role role) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;
}

/// <summary>
/// 角色更新事件
/// </summary>
public class RoleUpdatedEvent(Role role) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;
}

/// <summary>
/// 角色权限添加事件
/// </summary>
public class RolePermissionAddedEvent(Role role, Permission permission) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;

    /// <summary>
    /// 权限对象
    /// </summary>
    public Permission Permission { get; } = permission;
}

/// <summary>
/// 角色权限移除事件
/// </summary>
public class RolePermissionRemovedEvent(Role role, Permission permission) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;

    /// <summary>
    /// 权限对象
    /// </summary>
    public Permission Permission { get; } = permission;
}

/// <summary>
/// 角色权限批量变更事件
/// </summary>
public class RolePermissionsBulkChangedEvent(Role role, IEnumerable<Permission> addedPermissions, IEnumerable<Permission> removedPermissions) : DomainEvent
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;

    /// <summary>
    /// 新增的权限列表
    /// </summary>
    public IEnumerable<Permission> AddedPermissions { get; } = addedPermissions;

    /// <summary>
    /// 移除的权限列表
    /// </summary>
    public IEnumerable<Permission> RemovedPermissions { get; } = removedPermissions;
}