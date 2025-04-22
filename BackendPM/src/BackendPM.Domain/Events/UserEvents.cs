using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Events;

/// <summary>
/// 用户创建事件
/// </summary>
public class UserCreatedEvent : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; }
    
    public UserCreatedEvent(User user)
    {
        User = user;
    }
}

/// <summary>
/// 用户信息更新事件
/// </summary>
public class UserProfileUpdatedEvent : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; }
    
    /// <summary>
    /// 旧电子邮件
    /// </summary>
    public string OldEmail { get; }
    
    public UserProfileUpdatedEvent(User user, string oldEmail)
    {
        User = user;
        OldEmail = oldEmail;
    }
}

/// <summary>
/// 用户密码修改事件
/// </summary>
public class UserPasswordChangedEvent : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; }
    
    public UserPasswordChangedEvent(User user)
    {
        User = user;
    }
}

/// <summary>
/// 用户状态变更事件
/// </summary>
public class UserStatusChangedEvent : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; }
    
    /// <summary>
    /// 旧状态
    /// </summary>
    public bool OldStatus { get; }
    
    /// <summary>
    /// 新状态
    /// </summary>
    public bool NewStatus { get; }
    
    public UserStatusChangedEvent(User user, bool oldStatus, bool newStatus)
    {
        User = user;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}

/// <summary>
/// 用户添加角色事件
/// </summary>
public class UserRoleAddedEvent : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; }
    
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; }
    
    public UserRoleAddedEvent(User user, Role role)
    {
        User = user;
        Role = role;
    }
}

/// <summary>
/// 用户移除角色事件
/// </summary>
public class UserRoleRemovedEvent : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; }
    
    /// <summary>
    /// 角色对象
    /// </summary>
    public Role Role { get; }
    
    public UserRoleRemovedEvent(User user, Role role)
    {
        User = user;
        Role = role;
    }
}