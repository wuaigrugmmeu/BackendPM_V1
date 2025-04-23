// 此文件已不再需要，因为我们直接使用MediatR的接口
// 保留此文件仅作为历史记录，在确认项目稳定运行后可以删除

/*
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BackendPM.Application.Mediator;

/// <summary>
/// 请求处理器适配器
/// 注意：此适配器已不再需要，因为我们直接使用MediatR的接口
/// </summary>
public class RequestHandlerAdapter<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _handler;

    public RequestHandlerAdapter(IRequestHandler<TRequest, TResponse> handler)
    {
        _handler = handler;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        return await _handler.Handle(request, cancellationToken);
    }
}

/// <summary>
/// 无返回值请求处理器适配器
/// 注意：此适配器已不再需要，因为我们直接使用MediatR的接口
/// </summary>
public class RequestHandlerAdapter<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    private readonly IRequestHandler<TRequest> _handler;

    public RequestHandlerAdapter(IRequestHandler<TRequest> handler)
    {
        _handler = handler;
    }

    public async Task Handle(TRequest request, CancellationToken cancellationToken)
    {
        await _handler.Handle(request, cancellationToken);
    }
}
*/