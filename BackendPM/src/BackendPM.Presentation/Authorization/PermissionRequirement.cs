using Microsoft.AspNetCore.Authorization;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 基于权限的授权要求
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 权限编码
    /// </summary>
    public string PermissionCode { get; }
    
    /// <summary>
    /// 创建权限授权要求
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    public PermissionRequirement(string permissionCode)
    {
        PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
    }
}