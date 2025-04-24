namespace BackendPM.Application.DTOs;

/// <summary>
/// 菜单数据传输对象
/// </summary>
public class MenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 菜单路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 是否为系统菜单
    /// </summary>
    public bool IsSystem { get; set; }
    
    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentMenuId { get; set; }
    
    /// <summary>
    /// 父菜单名称
    /// </summary>
    public string? ParentMenuName { get; set; }
    
    /// <summary>
    /// 子菜单列表
    /// </summary>
    public List<MenuBasicDto> ChildMenus { get; set; } = [];
    
    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; }

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
/// 菜单基本信息DTO，用于列表显示
/// </summary>
public class MenuBasicDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 菜单编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// 菜单路径
    /// </summary>
    public string? Path { get; set; }
    
    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; }
}

/// <summary>
/// 角色菜单DTO
/// </summary>
public class RoleMenuDto
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 菜单编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 菜单路径
    /// </summary>
    public string? Path { get; set; }
    
    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }
}