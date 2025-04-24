namespace BackendPM.Domain.Entities;

/// <summary>
/// 角色和权限的关联实体
/// </summary>
public class RolePermission
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid PermissionId { get; private set; }

    /// <summary>
    /// 关联的角色
    /// </summary>
    public virtual Role Role { get; private set; }

    /// <summary>
    /// 关联的权限
    /// </summary>
    public virtual Permission Permission { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // 无参构造函数用于EF Core
    private RolePermission()
    {
        Role = null!;
        Permission = null!;
    }

    /// <summary>
    /// 创建角色-权限关联
    /// </summary>
    public RolePermission(Role role, Permission permission)
    {
        RoleId = role.Id;
        PermissionId = permission.Id;
        Role = role;
        Permission = permission;
        CreatedAt = DateTime.UtcNow;
    }
}