namespace BackendPM.Domain.Entities;

/// <summary>
/// 用户和角色的关联实体
/// </summary>
public class UserRole
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }
    
    /// <summary>
    /// 关联的用户
    /// </summary>
    public virtual User User { get; private set; }
    
    /// <summary>
    /// 关联的角色
    /// </summary>
    public virtual Role Role { get; private set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // 无参构造函数用于EF Core
    private UserRole()
    {
        User = null!;
        Role = null!;
    }

    /// <summary>
    /// 创建用户-角色关联
    /// </summary>
    public UserRole(User user, Role role)
    {
        UserId = user.Id;
        RoleId = role.Id;
        User = user;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}