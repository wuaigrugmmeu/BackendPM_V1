using System;
using MediatR;

namespace BackendPM.Application.Commands;

/// <summary>
/// 不返回结果的基础命令
/// </summary>
public abstract class BaseCommand : IRequest
{
    /// <summary>
    /// 命令ID
    /// </summary>
    public Guid CommandId { get; }
    
    /// <summary>
    /// 命令创建时间
    /// </summary>
    public DateTime CreatedAt { get; }
    
    protected BaseCommand()
    {
        CommandId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// 返回结果的基础命令
/// </summary>
/// <typeparam name="TResult">返回结果类型</typeparam>
public abstract class BaseCommand<TResult> : IRequest<TResult>
{
    /// <summary>
    /// 命令ID
    /// </summary>
    public Guid CommandId { get; }
    
    /// <summary>
    /// 命令创建时间
    /// </summary>
    public DateTime CreatedAt { get; }
    
    protected BaseCommand()
    {
        CommandId = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}