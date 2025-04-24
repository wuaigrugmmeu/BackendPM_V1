using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendPM.Domain.Interfaces.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Infrastructure.InfrastructureServices.DomainEvents;

/// <summary>
/// 领域事件通知适配器 - 将领域事件转换为中介者通知
/// </summary>
public class DomainEventNotificationAdapter(IMediator mediator, ILogger<DomainEventNotificationAdapter> logger) : IDomainEventDispatcher
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DomainEventNotificationAdapter> _logger = logger;

    /// <summary>
    /// 分发单个领域事件
    /// </summary>
    public async Task DispatchAsync(IDomainEvent @event)
    {
        var eventName = @event.GetType().Name;
        _logger.LogInformation("通过中介者分发领域事件: {EventName} ({EventId})", eventName, @event.EventId);

        try
        {
            // IDomainEvent 现在已经继承自 INotification，可以直接发布
            await _mediator.Publish(@event);
            _logger.LogInformation("领域事件通过中介者分发成功: {EventName} ({EventId})", eventName, @event.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "通过中介者分发领域事件时出错: {EventName} ({EventId})", eventName, @event.EventId);
            throw;
        }
    }

    /// <summary>
    /// 分发多个领域事件
    /// </summary>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
    {
        var eventList = events.ToList();
        _logger.LogInformation("通过中介者分发 {Count} 个领域事件", eventList.Count);

        foreach (var @event in eventList)
        {
            await DispatchAsync(@event);
        }
    }
}