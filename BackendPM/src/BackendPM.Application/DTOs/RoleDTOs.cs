using System;
using System.Collections.Generic;

namespace BackendPM.Application.DTOs;

/// <summary>
/// 角色数据传输对象
/// </summary>
public class RoleDto
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
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 是否为系统角色
    /// </summary>
    public bool IsSystemRole { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }
    
    /// <summary>
    /// 角色拥有的权限列表
    /// </summary>
    public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
}

/// <summary>
/// 权限数据传输对象
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// 权限ID
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 权限名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 权限编码
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// 权限分组
    /// </summary>
    public string Group { get; set; } = string.Empty;
    
    /// <summary>
    /// 权限描述
    /// </summary>
    public string? Description { get; set; }
}