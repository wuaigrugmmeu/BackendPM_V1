using Microsoft.AspNetCore.Authorization;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 基于权限的授权要求
/// </summary>
/// <remarks>
/// 创建权限授权要求
/// </remarks>
/// <param name="permissionCode">权限编码</param>
public class PermissionRequirement(string permissionCode) : IAuthorizationRequirement
{
    /// <summary>
    /// 权限编码
    /// </summary>
    public string PermissionCode { get; } = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
}