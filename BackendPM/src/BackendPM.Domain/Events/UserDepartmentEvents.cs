using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Events;

/// <summary>
/// 用户部门添加事件
/// </summary>
public class UserDepartmentAddedEvent(User user, Department department) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 部门对象
    /// </summary>
    public Department Department { get; } = department;
}

/// <summary>
/// 用户部门移除事件
/// </summary>
public class UserDepartmentRemovedEvent(User user, Department department) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 部门对象
    /// </summary>
    public Department Department { get; } = department;
}

/// <summary>
/// 用户部门设为主部门事件
/// </summary>
public class UserDepartmentSetPrimaryEvent(User user, Department department) : DomainEvent
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public User User { get; } = user;

    /// <summary>
    /// 部门对象
    /// </summary>
    public Department Department { get; } = department;
}