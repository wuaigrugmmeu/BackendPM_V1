using BackendPM.Domain.Exceptions;

namespace BackendPM.Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class Role : EntityBase
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 角色编码，用于系统识别
    /// </summary>
    public string Code { get; private set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// 是否为系统角色（系统角色不能被删除）
    /// </summary>
    public bool IsSystem { get; private set; }
    
    /// <summary>
    /// 角色关联的用户列表
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; private set; }
    
    /// <summary>
    /// 角色拥有的权限列表
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; private set; }
    
    // 无参构造函数用于EF Core
    private Role()
    {
        Name = string.Empty;
        Code = string.Empty;
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
    }
    
    /// <summary>
    /// 创建一个新角色
    /// </summary>
    public Role(string name, string code, string? description = null, bool isSystem = false)
    {
        ValidateName(name);
        ValidateCode(code);
        
        Name = name;
        Code = code;
        Description = description;
        IsSystem = isSystem;
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
    }
    
    /// <summary>
    /// 更新角色信息
    /// </summary>
    public void Update(string name, string? description)
    {
        ValidateName(name);
        
        Name = name;
        Description = description;
        UpdateModificationTime();
    }
    
    /// <summary>
    /// 添加权限到角色
    /// </summary>
    public void AddPermission(Permission permission)
    {
        if (RolePermissions.Any(rp => rp.PermissionId == permission.Id))
            return;
            
        RolePermissions.Add(new RolePermission(this, permission));
        UpdateModificationTime();
    }
    
    /// <summary>
    /// 从角色中移除权限
    /// </summary>
    public void RemovePermission(Permission permission)
    {
        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permission.Id);
        if (rolePermission != null)
        {
            RolePermissions.Remove(rolePermission);
            UpdateModificationTime();
        }
    }
    
    /// <summary>
    /// 批量设置角色权限
    /// </summary>
    /// <param name="permissions">要设置的权限列表</param>
    /// <remarks>
    /// 此方法会清除当前角色的所有权限，然后添加指定的权限
    /// </remarks>
    public void SetPermissions(IEnumerable<Permission> permissions)
    {
        // 如果是系统角色且权限列表为空，则抛出异常
        if (IsSystem && (permissions == null || !permissions.Any()))
        {
            throw new BusinessRuleViolationException("系统角色不能设置为无权限");
        }
        
        // 清除当前所有权限
        RolePermissions.Clear();
        
        // 添加新的权限
        if (permissions != null)
        {
            foreach (var permission in permissions)
            {
                RolePermissions.Add(new RolePermission(this, permission));
            }
        }
        
        UpdateModificationTime();
    }
    
    /// <summary>
    /// 批量添加权限
    /// </summary>
    /// <param name="permissions">要添加的权限列表</param>
    public void AddPermissions(IEnumerable<Permission> permissions)
    {
        if (permissions == null)
            return;
            
        bool changed = false;
        foreach (var permission in permissions)
        {
            if (!RolePermissions.Any(rp => rp.PermissionId == permission.Id))
            {
                RolePermissions.Add(new RolePermission(this, permission));
                changed = true;
            }
        }
        
        if (changed)
        {
            UpdateModificationTime();
        }
    }
    
    /// <summary>
    /// 检查角色是否拥有指定权限
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    /// <returns>是否拥有权限</returns>
    public bool HasPermission(string permissionCode)
    {
        return RolePermissions.Any(rp => rp.Permission.Code == permissionCode);
    }
    
    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("角色名称不能为空", nameof(name));
            
        if (name.Length > 50)
            throw new ArgumentException("角色名称过长", nameof(name));
    }
    
    private void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("角色编码不能为空", nameof(code));
            
        if (code.Length > 50)
            throw new ArgumentException("角色编码过长", nameof(code));
    }
}