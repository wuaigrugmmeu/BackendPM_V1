using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BackendPM.Application.Mediator;

/// <summary>
/// MediatR适配器
/// </summary>
public class MediatorAdapter 
{
    private readonly IMediator _mediator;

    public MediatorAdapter(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 发送请求并等待响应
    /// </summary>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    /// 发送不需要响应的请求
    /// </summary>
    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request, cancellationToken);
    }

    /// <summary>
    /// 发布通知
    /// </summary>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        await _mediator.Publish(notification, cancellationToken);
    }
}