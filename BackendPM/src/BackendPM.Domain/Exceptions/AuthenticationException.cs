namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 认证异常
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="message">异常消息</param>
public class AuthenticationException(string message) : DomainException(message, "AuthenticationFailed")
{
}