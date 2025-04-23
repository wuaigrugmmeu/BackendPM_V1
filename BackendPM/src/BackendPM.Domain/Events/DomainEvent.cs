using System;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Events;
using MediatR;

namespace BackendPM.Domain.Events;

/// <summary>
/// 领域事件基类
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public Guid EventId { get; }
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; }
    
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}