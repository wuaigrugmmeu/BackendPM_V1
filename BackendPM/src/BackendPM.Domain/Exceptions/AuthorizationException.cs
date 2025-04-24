namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 授权异常
/// </summary>
public class AuthorizationException : DomainException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public AuthorizationException(string message)
        : base(message, "AuthorizationFailed")
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="resource">资源名称</param>
    /// <param name="permission">所需权限</param>
    public AuthorizationException(string resource, string permission)
        : base($"用户没有对资源 {resource} 执行 {permission} 操作的权限", "AuthorizationFailed")
    {
    }
}