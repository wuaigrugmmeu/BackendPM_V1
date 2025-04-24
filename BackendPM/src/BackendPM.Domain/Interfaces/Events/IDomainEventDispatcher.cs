namespace BackendPM.Domain.Interfaces.Events;

/// <summary>
/// 领域事件分发器接口
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// 分发领域事件
    /// </summary>
    /// <param name="event">领域事件</param>
    Task DispatchAsync(IDomainEvent @event);

    /// <summary>
    /// 分发多个领域事件
    /// </summary>
    /// <param name="events">领域事件列表</param>
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}