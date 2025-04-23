namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 认证异常
/// </summary>
public class AuthenticationException : DomainException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public AuthenticationException(string message) 
        : base(message, "AuthenticationFailed")
    {
    }
}