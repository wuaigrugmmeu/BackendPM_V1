using MediatR;

namespace BackendPM.Domain.Interfaces.Events;

/// <summary>
/// 领域事件接口
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// 事件发生时间戳
    /// </summary>
    DateTime OccurredOn { get; }
    
    /// <summary>
    /// 事件唯一标识
    /// </summary>
    Guid EventId { get; }
}