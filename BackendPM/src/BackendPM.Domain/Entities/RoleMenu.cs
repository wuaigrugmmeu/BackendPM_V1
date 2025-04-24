namespace BackendPM.Domain.Entities;

/// <summary>
/// 角色和菜单的关联实体
/// </summary>
public class RoleMenu
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; private set; }

    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; private set; }

    /// <summary>
    /// 关联的角色
    /// </summary>
    public virtual Role Role { get; private set; }

    /// <summary>
    /// 关联的菜单
    /// </summary>
    public virtual Menu Menu { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // 无参构造函数用于EF Core
    private RoleMenu()
    {
        Role = null!;
        Menu = null!;
    }

    /// <summary>
    /// 创建角色-菜单关联
    /// </summary>
    public RoleMenu(Role role, Menu menu)
    {
        RoleId = role.Id;
        MenuId = menu.Id;
        Role = role;
        Menu = menu;
        CreatedAt = DateTime.UtcNow;
    }
}