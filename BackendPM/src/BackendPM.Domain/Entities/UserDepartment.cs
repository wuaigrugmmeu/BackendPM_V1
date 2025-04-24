namespace BackendPM.Domain.Entities;

/// <summary>
/// 用户和部门的关联实体
/// </summary>
public class UserDepartment
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid DepartmentId { get; private set; }

    /// <summary>
    /// 关联的用户
    /// </summary>
    public virtual User User { get; private set; }

    /// <summary>
    /// 关联的部门
    /// </summary>
    public virtual Department Department { get; private set; }

    /// <summary>
    /// 是否为主部门
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    // 无参构造函数用于EF Core
    private UserDepartment()
    {
        User = null!;
        Department = null!;
    }

    /// <summary>
    /// 创建用户-部门关联
    /// </summary>
    public UserDepartment(User user, Department department, bool isPrimary = false)
    {
        UserId = user.Id;
        DepartmentId = department.Id;
        User = user;
        Department = department;
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 设置为主部门
    /// </summary>
    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}