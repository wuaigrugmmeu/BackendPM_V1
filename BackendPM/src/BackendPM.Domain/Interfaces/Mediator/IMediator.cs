// 此文件中的接口已不再需要，因为我们直接使用MediatR的接口
// 保留此文件仅作为历史记录，在确认项目稳定运行后可以删除

/*
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Domain.Interfaces.Mediator;

/// <summary>
/// 中介者接口 - 负责请求的分发和处理
/// </summary>
public interface IMediator
{
    /// <summary>
    /// 发送请求并等待响应
    /// </summary>
    /// <typeparam name="TResponse">响应类型</typeparam>
    /// <param name="request">请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>请求的处理结果</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 发送不需要响应的请求
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task Send(IRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 发布通知
    /// </summary>
    /// <typeparam name="TNotification">通知类型</typeparam>
    /// <param name="notification">通知</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
*/