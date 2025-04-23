using BackendPM.Domain.Exceptions;

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
    /// 资源类型（如API、菜单、按钮等）
    /// </summary>
    public string ResourceType { get; private set; }
    
    /// <summary>
    /// 资源路径，用于API权限时指定接口路径
    /// </summary>
    public string? ResourcePath { get; private set; }
    
    /// <summary>
    /// HTTP方法，适用于API资源类型
    /// </summary>
    public string? HttpMethod { get; private set; }
    
    /// <summary>
    /// 排序序号，用于在UI中的显示顺序
    /// </summary>
    public int SortOrder { get; private set; }
    
    /// <summary>
    /// 是否为系统内置权限（内置权限不可删除）
    /// </summary>
    public bool IsSystem { get; private set; }
    
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
        ResourceType = "Other";
        RolePermissions = new List<RolePermission>();
    }
    
    /// <summary>
    /// 创建一个新权限
    /// </summary>
    public Permission(string name, string code, string group, string? description = null, 
                     string resourceType = "Other", string? resourcePath = null, 
                     string? httpMethod = null, int sortOrder = 0, bool isSystem = false)
    {
        ValidateName(name);
        ValidateCode(code);
        ValidateGroup(group);
        
        Name = name;
        Code = code;
        Group = group;
        Description = description;
        ResourceType = resourceType;
        ResourcePath = resourcePath;
        HttpMethod = httpMethod;
        SortOrder = sortOrder;
        IsSystem = isSystem;
        RolePermissions = new List<RolePermission>();
    }
    
    /// <summary>
    /// 更新权限信息
    /// </summary>
    public void Update(string name, string group, string? description, string resourceType,
                      string? resourcePath, string? httpMethod, int sortOrder)
    {
        if (IsSystem)
        {
            throw new BusinessRuleViolationException("系统内置权限不允许修改基本信息");
        }
        
        ValidateName(name);
        ValidateGroup(group);
        
        Name = name;
        Group = group;
        Description = description;
        ResourceType = resourceType;
        ResourcePath = resourcePath;
        HttpMethod = httpMethod;
        SortOrder = sortOrder;
        UpdateModificationTime();
    }
    
    // 验证方法保持不变
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