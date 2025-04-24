using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Events;

/// <summary>
/// 用户创建事件
/// </summary>
public class UserCreatedEvent(User user) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;
}

/// <summary>
/// 用户信息更新事件
/// </summary>
public class UserProfileUpdatedEvent(User user, string oldEmail) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 旧电子邮件
    /// </summary>
    public string OldEmail { get; } = oldEmail;
}

/// <summary>
/// 用户密码修改事件
/// </summary>
public class UserPasswordChangedEvent(User user) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;
}

/// <summary>
/// 用户状态变更事件
/// </summary>
public class UserStatusChangedEvent(User user, bool oldStatus, bool newStatus) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 旧状态
    /// </summary>
    public bool OldStatus { get; } = oldStatus;

    /// <summary>
    /// 新状态
    /// </summary>
    public bool NewStatus { get; } = newStatus;
}

/// <summary>
/// 用户添加角色事件
/// </summary>
public class UserRoleAddedEvent(User user, Role role) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;
}

/// <summary>
/// 用户移除角色事件
/// </summary>
public class UserRoleRemovedEvent(User user, Role role) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; } = role;
}