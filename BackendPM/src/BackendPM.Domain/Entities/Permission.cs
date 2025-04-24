using BackendPM.Domain.Constants;
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
            throw new BusinessRuleViolationException(ErrorMessages.Permission.SystemPermissionModificationForbidden);
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
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(ErrorMessages.Permission.NameRequired, nameof(name));

        if (name.Length > 50)
            throw new ArgumentException(ErrorMessages.Permission.NameTooLong, nameof(name));
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(ErrorMessages.Permission.CodeRequired, nameof(code));

        if (code.Length > 100)
            throw new ArgumentException(ErrorMessages.Permission.CodeTooLong, nameof(code));
    }

    private static void ValidateGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
            throw new ArgumentException(ErrorMessages.Permission.GroupRequired, nameof(group));

        if (group.Length > 50)
            throw new ArgumentException(ErrorMessages.Permission.GroupTooLong, nameof(group));
    }
}