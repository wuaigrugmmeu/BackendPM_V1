namespace BackendPM.Domain.Entities;

/// <summary>
/// 刷新令牌实体
/// </summary>
public class RefreshToken : EntityBase
{
    /// <summary>
    /// 关联的用户ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 令牌值
    /// </summary>
    public string Token { get; private set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime ExpiryTime { get; private set; }

    /// <summary>
    /// 是否已使用
    /// </summary>
    public bool IsUsed { get; private set; }

    /// <summary>
    /// 是否已撤销
    /// </summary>
    public bool IsRevoked { get; private set; }

    /// <summary>
    /// 用户关联
    /// </summary>
    public virtual User? User { get; }

    // 无参构造函数用于EF Core
    private RefreshToken()
    {
        Token = string.Empty;
    }

    /// <summary>
    /// 创建新的刷新令牌
    /// </summary>
    public RefreshToken(Guid userId, string token, DateTime expiryTime)
    {
        UserId = userId;
        Token = token;
        ExpiryTime = expiryTime;
        IsUsed = false;
        IsRevoked = false;
    }

    /// <summary>
    /// 标记令牌为已使用
    /// </summary>
    public void MarkAsUsed()
    {
        IsUsed = true;
        UpdateModificationTime();
    }

    /// <summary>
    /// 撤销令牌
    /// </summary>
    public void Revoke()
    {
        IsRevoked = true;
        UpdateModificationTime();
    }

    /// <summary>
    /// 检查令牌是否有效
    /// </summary>
    public bool IsActive => !IsUsed && !IsRevoked && ExpiryTime > DateTime.UtcNow;
}