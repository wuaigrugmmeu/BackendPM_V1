using System.Collections.Concurrent;
using BackendPM.Domain.Interfaces.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BackendPM.Infrastructure.InfrastructureServices.DomainEvents;

/// <summary>
/// 领域事件分发器实现
/// </summary>
public class DomainEventDispatcher(IMediator mediator, ILogger<DomainEventDispatcher> logger) : IDomainEventDispatcher
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DomainEventDispatcher> _logger = logger;

    /// <summary>
    /// 分发单个领域事件
    /// </summary>
    public async Task DispatchAsync(IDomainEvent @event)
    {
        var eventName = @event.GetType().Name;
        _logger.LogInformation("正在分发领域事件: {EventName} ({EventId})", eventName, @event.EventId);

        try
        {
            // 使用MediatR发布领域事件
            await _mediator.Publish(@event);
            _logger.LogInformation("领域事件分发成功: {EventName} ({EventId})", eventName, @event.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分发领域事件时出错: {EventName} ({EventId})", eventName, @event.EventId);
            throw;
        }
    }

    /// <summary>
    /// 分发多个领域事件
    /// </summary>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
    {
        var eventList = events.ToList();
        _logger.LogInformation("正在分发 {Count} 个领域事件", eventList.Count);

        foreach (var @event in eventList)
        {
            await DispatchAsync(@event);
        }
    }
}