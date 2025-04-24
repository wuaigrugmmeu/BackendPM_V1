using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Events;

/// <summary>
/// 部门创建事件
/// </summary>
public class DepartmentCreatedEvent(Department department) : DomainEvent
{
    /// <summary>
    /// 部门对象
    /// </summary>
    public Department Department { get; } = department;
}

/// <summary>
/// 部门更新事件
/// </summary>
public class DepartmentUpdatedEvent(Department department) : DomainEvent
{
    /// <summary>
    /// 部门对象
    /// </summary>
    public Department Department { get; } = department;
}

/// <summary>
/// 部门删除事件
/// </summary>
public class DepartmentDeletedEvent(Department department) : DomainEvent
{
    /// <summary>
    /// 部门对象
    /// </summary>
    public Department Department { get; } = department;
}