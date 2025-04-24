using BackendPM.Domain.Interfaces.Events;

namespace BackendPM.Domain.Interfaces;

/// <summary>
/// 聚合根接口
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// 获取未处理的领域事件
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// 移除领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    void RemoveDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// 清除所有领域事件
    /// </summary>
    void ClearDomainEvents();
}