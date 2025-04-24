using System;
using MediatR;

namespace BackendPM.Application.Queries;

/// <summary>
/// 基础查询
/// </summary>
/// <typeparam name="TResult">查询结果类型</typeparam>
public abstract class BaseQuery<TResult> : IRequest<TResult>
{
    /// <summary>
    /// 查询ID
    /// </summary>
    public Guid QueryId { get; }

    /// <summary>
    /// 查询创建时间
    /// </summary>
    public DateTime CreatedAt { get; }

    protected BaseQuery()
    {
        QueryId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}