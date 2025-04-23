namespace BackendPM.Domain.Exceptions;

/// <summary>
/// 业务规则违反异常
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public BusinessRuleViolationException(string message) 
        : base(message, "BusinessRuleViolation")
    {
    }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="specificCode">特定的错误代码</param>
    public BusinessRuleViolationException(string message, string specificCode) 
        : base(message, specificCode)
    {
    }
}