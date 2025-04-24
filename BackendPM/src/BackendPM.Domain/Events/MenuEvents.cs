using BackendPM.Domain.Entities;

namespace BackendPM.Domain.Events;

/// <summary>
/// 菜单创建事件
/// </summary>
public class MenuCreatedEvent(Menu menu) : DomainEvent
{
    /// <summary>
    /// 菜单对象
    /// </summary>
    public Menu Menu { get; } = menu;
}

/// <summary>
/// 菜单更新事件
/// </summary>
public class MenuUpdatedEvent(Menu menu) : DomainEvent
{
    /// <summary>
    /// 菜单对象
    /// </summary>
    public Menu Menu { get; } = menu;
}

/// <summary>
/// 菜单删除事件
/// </summary>
public class MenuDeletedEvent(Menu menu) : DomainEvent
{
    /// <summary>
    /// 菜单对象
    /// </summary>
    public Menu Menu { get; } = menu;
}