namespace BackendPM.Domain.Entities;

/// <summary>
/// 权限实体
/// </summary>
public class Permission : EntityBase
{
    /// <summary>
    /// 权限名称
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 权限编码，用于系统识别
    /// </summary>
    public string Code { get; private set; }
    
    /// <summary>
    /// 权限分组，用于组织和显示
    /// </summary>
    public string Group { get; private set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// 关联的角色权限列表
    /// </summary>
    public virtual ICollection<RolePermission> RolePermissions { get; private set; }
    
    // 无参构造函数用于EF Core
    private Permission()
    {
        Name = string.Empty;
        Code = string.Empty;
        Group = string.Empty;
        RolePermissions = new List<RolePermission>();
    }
    
    /// <summary>
    /// 创建一个新权限
    /// </summary>
    public Permission(string name, string code, string group, string? description = null)
    {
        ValidateName(name);
        ValidateCode(code);
        ValidateGroup(group);
        
        Name = name;
        Code = code;
        Group = group;
        Description = description;
        RolePermissions = new List<RolePermission>();
    }
    
    /// <summary>
    /// 更新权限信息
    /// </summary>
    public void Update(string name, string group, string? description)
    {
        ValidateName(name);
        ValidateGroup(group);
        
        Name = name;
        Group = group;
        Description = description;
        UpdateModificationTime();
    }
    
    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("权限名称不能为空", nameof(name));
            
        if (name.Length > 50)
            throw new ArgumentException("权限名称过长", nameof(name));
    }
    
    private void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("权限编码不能为空", nameof(code));
            
        if (code.Length > 100)
            throw new ArgumentException("权限编码过长", nameof(code));
    }
    
    private void ValidateGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException("权限分组不能为空", nameof(group));
            
        if (group.Length > 50)
            throw new ArgumentException("权限分组名称过长", nameof(group));
    }
}