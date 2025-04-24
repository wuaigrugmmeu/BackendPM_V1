using System.ComponentModel.DataAnnotations;

namespace BackendPM.Application.DTOs;

/// <summary>
/// 用户DTO
/// </summary>
public class UserDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 电子邮件
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// 用户角色名称列表（保留向后兼容性）
    /// </summary>
    public List<string> Roles { get; set; } = [];
    
    /// <summary>
    /// 用户角色详细信息
    /// </summary>
    public List<UserRoleDto> UserRoles { get; set; } = [];
    
    /// <summary>
    /// 用户部门详细信息
    /// </summary>
    public List<UserDepartmentDto> UserDepartments { get; set; } = [];
    
    /// <summary>
    /// 主部门信息
    /// </summary>
    public UserDepartmentDto? PrimaryDepartment { get; set; }
}

/// <summary>
/// 用户角色DTO
/// </summary>
public class UserRoleDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否为系统角色
    /// </summary>
    public bool IsSystemRole { get; set; }
}

/// <summary>
/// 创建用户请求DTO
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名是必填项")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度必须在3-50个字符之间")]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 电子邮件
    /// </summary>
    [Required(ErrorMessage = "电子邮件是必填项")]
    [EmailAddress(ErrorMessage = "电子邮件格式不正确")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码是必填项")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 全名
    /// </summary>
    [StringLength(100, ErrorMessage = "全名长度不能超过100个字符")]
    public string? FullName { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<Guid>? RoleIds { get; set; }
    
    /// <summary>
    /// 部门ID列表
    /// </summary>
    public List<Guid>? DepartmentIds { get; set; }
    
    /// <summary>
    /// 主部门ID
    /// </summary>
    public Guid? PrimaryDepartmentId { get; set; }
}

/// <summary>
/// 更新用户请求DTO
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// 电子邮件
    /// </summary>
    [EmailAddress(ErrorMessage = "电子邮件格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 全名
    /// </summary>
    [StringLength(100, ErrorMessage = "全名长度不能超过100个字符")]
    public string? FullName { get; set; }

    /// <summary>
    /// 是否活跃
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// 更改密码请求DTO
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// 当前密码
    /// </summary>
    [Required(ErrorMessage = "当前密码是必填项")]
    public string CurrentPassword { get; set; } = null!;

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "新密码是必填项")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string NewPassword { get; set; } = null!;

    /// <summary>
    /// 确认新密码
    /// </summary>
    [Required(ErrorMessage = "确认新密码是必填项")]
    [Compare("NewPassword", ErrorMessage = "两次输入的密码不一致")]
    public string ConfirmNewPassword { get; set; } = null!;
}

/// <summary>
/// 用户分页查询参数
/// </summary>
public class UserQueryParams
{
    /// <summary>
    /// 页码，从1开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 搜索关键词
    /// </summary>
    public string? SearchTerm { get; set; }
}

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">列表项类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页记录数
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = [];
}