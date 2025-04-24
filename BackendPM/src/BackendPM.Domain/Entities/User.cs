using System.ComponentModel.DataAnnotations;
using BackendPM.Domain.Events;

namespace BackendPM.Domain.Entities;

/// <summary>
/// 用户实体
/// </summary>
public class User : AggregateRoot
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; private set; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 用户的角色列表
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; private set; }
    
    /// <summary>
    /// 用户的部门列表
    /// </summary>
    public virtual ICollection<UserDepartment> UserDepartments { get; private set; }

    // 无参构造函数用于EF Core
    private User()
    {
        Username = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        UserRoles = new List<UserRole>();
        UserDepartments = new List<UserDepartment>();
    }

    /// <summary>
    /// 创建一个新用户
    /// </summary>
    public User(string username, string email, string passwordHash)
    {
        ValidateUsername(username);
        ValidateEmail(email);

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
        UserRoles = new List<UserRole>();
        UserDepartments = new List<UserDepartment>();

        // 添加用户创建事件
        AddDomainEvent(new UserCreatedEvent(this));
    }

    /// <summary>
    /// 修改用户信息
    /// </summary>
    public void UpdateProfile(string email, string? fullName)
    {
        ValidateEmail(email);

        string oldEmail = Email;
        Email = email;
        FullName = fullName;
        UpdateModificationTime();

        // 添加用户信息更新事件
        AddDomainEvent(new UserProfileUpdatedEvent(this, oldEmail));
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("密码哈希不能为空", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdateModificationTime();

        // 添加密码修改事件
        AddDomainEvent(new UserPasswordChangedEvent(this));
    }

    /// <summary>
    /// 更新用户密码
    /// </summary>
    public void UpdatePassword(string passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
            throw new ArgumentNullException(nameof(passwordHash));

        PasswordHash = passwordHash;
        UpdateModificationTime();

        // 添加密码修改事件
        AddDomainEvent(new UserPasswordChangedEvent(this));
    }

    /// <summary>
    /// 激活或停用用户
    /// </summary>
    public void SetActiveStatus(bool isActive)
    {
        bool oldStatus = IsActive;
        IsActive = isActive;
        UpdateModificationTime();

        // 添加用户状态变更事件
        if (oldStatus != isActive)
        {
            AddDomainEvent(new UserStatusChangedEvent(this, oldStatus, isActive));
        }
    }

    /// <summary>
    /// 添加用户角色
    /// </summary>
    public void AddRole(Role role)
    {
        if (UserRoles.Any(ur => ur.RoleId == role.Id))
            return;

        UserRoles.Add(new UserRole(this, role));
        UpdateModificationTime();

        // 添加用户角色变更事件
        AddDomainEvent(new UserRoleAddedEvent(this, role));
    }

    /// <summary>
    /// 移除用户角色
    /// </summary>
    public void RemoveRole(Role role)
    {
        var userRole = UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole != null)
        {
            UserRoles.Remove(userRole);
            UpdateModificationTime();

            // 添加用户角色变更事件
            AddDomainEvent(new UserRoleRemovedEvent(this, role));
        }
    }

    /// <summary>
    /// 添加用户部门
    /// </summary>
    public void AddDepartment(Department department, bool isPrimary = false)
    {
        // 如果添加为主部门，需要先将其他部门设置为非主部门
        if (isPrimary)
        {
            foreach (var userDepartment in UserDepartments.Where(ud => ud.IsPrimary))
            {
                userDepartment.SetAsPrimary(false);
            }
        }

        // 检查是否已经添加了该部门
        if (UserDepartments.Any(ud => ud.DepartmentId == department.Id))
        {
            // 如果部门已存在，只需更新是否为主部门
            var existingUserDepartment = UserDepartments.First(ud => ud.DepartmentId == department.Id);
            existingUserDepartment.SetAsPrimary(isPrimary);
            return;
        }

        UserDepartments.Add(new UserDepartment(this, department, isPrimary));
        UpdateModificationTime();

        // 添加用户部门变更事件
        AddDomainEvent(new UserDepartmentAddedEvent(this, department));
    }

    /// <summary>
    /// 移除用户部门
    /// </summary>
    public void RemoveDepartment(Department department)
    {
        var userDepartment = UserDepartments.FirstOrDefault(ud => ud.DepartmentId == department.Id);
        if (userDepartment != null)
        {
            bool isPrimary = userDepartment.IsPrimary;
            UserDepartments.Remove(userDepartment);
            UpdateModificationTime();

            // 如果移除的是主部门且还有其他部门，将第一个设为主部门
            if (isPrimary && UserDepartments.Any())
            {
                UserDepartments.First().SetAsPrimary(true);
            }

            // 添加用户部门变更事件
            AddDomainEvent(new UserDepartmentRemovedEvent(this, department));
        }
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("用户名不能为空", nameof(username));

        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException("用户名长度必须在3-50个字符之间", nameof(username));
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("邮箱不能为空", nameof(email));

        if (!new EmailAddressAttribute().IsValid(email))
            throw new ArgumentException("邮箱格式不正确", nameof(email));
    }
}