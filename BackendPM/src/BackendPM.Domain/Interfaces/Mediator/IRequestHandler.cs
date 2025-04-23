// 此文件中的接口已不再需要，因为我们直接使用MediatR的接口
// 保留此文件仅作为历史记录，在确认项目稳定运行后可以删除

/*
using System.Threading;
using System.Threading.Tasks;

namespace BackendPM.Domain.Interfaces.Mediator;

/// <summary>
/// 请求处理器接口 - 处理不返回值的请求
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
public interface IRequestHandler<in TRequest> : MediatR.IRequestHandler<TRequest> 
    where TRequest : IRequest
{
    // 继承自 MediatR.IRequestHandler<TRequest> 的 Handle 方法
    // 不需要在这里重新声明
}

/// <summary>
/// 请求处理器接口 - 处理返回值的请求
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">返回值类型</typeparam>
public interface IRequestHandler<in TRequest, TResponse> : MediatR.IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    // 继承自 MediatR.IRequestHandler<TRequest, TResponse> 的 Handle 方法
    // 不需要在这里重新声明
}
*/