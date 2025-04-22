using BackendPM.Domain.Interfaces.Events;

namespace BackendPM.Domain.Events;

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; }
    
    /// <summary>
    /// 事件唯一标识
    /// </summary>
    public Guid EventId { get; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}