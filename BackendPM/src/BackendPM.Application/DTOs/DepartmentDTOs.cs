namespace BackendPM.Application.DTOs;

/// <summary>
/// 部门数据传输对象
/// </summary>
public class DepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 部门描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否为系统部门
    /// </summary>
    public bool IsSystem { get; set; }
    
    /// <summary>
    /// 父部门ID
    /// </summary>
    public Guid? ParentDepartmentId { get; set; }
    
    /// <summary>
    /// 父部门名称
    /// </summary>
    public string? ParentDepartmentName { get; set; }
    
    /// <summary>
    /// 子部门列表
    /// </summary>
    public List<DepartmentBasicDto> ChildDepartments { get; set; } = [];
    
    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }
}

/// <summary>
/// 部门基本信息DTO，用于列表显示
/// </summary>
public class DepartmentBasicDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 部门编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// 用户部门DTO
/// </summary>
public class UserDepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 部门名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 部门编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否为主部门
    /// </summary>
    public bool IsPrimary { get; set; }
}