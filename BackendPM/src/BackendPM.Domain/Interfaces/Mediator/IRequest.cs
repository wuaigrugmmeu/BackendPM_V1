// 此文件中的接口已不再需要，因为我们直接使用MediatR的接口
// 保留此文件仅作为历史记录，在确认项目稳定运行后可以删除

/*
using System;

namespace BackendPM.Domain.Interfaces.Mediator;

/// <summary>
/// 请求接口 - 不携带返回值的请求
/// </summary>
public interface IRequest : MediatR.IRequest
{
}

/// <summary>
/// 请求接口 - 携带返回值的请求
/// </summary>
/// <typeparam name="TResponse">返回值类型</typeparam>
public interface IRequest<out TResponse> : MediatR.IRequest<TResponse>
{
}
*/