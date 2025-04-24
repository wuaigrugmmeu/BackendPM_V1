using System;

namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 领域异常基类
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="code">错误代码</param>
    protected DomainException(string message, string code) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="code">错误代码</param>
    /// <param name="innerException">内部异常</param>
    protected DomainException(string message, string code, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}