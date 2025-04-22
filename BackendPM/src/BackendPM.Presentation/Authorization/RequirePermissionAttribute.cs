using Microsoft.AspNetCore.Authorization;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 要求特定权限的授权特性
/// </summary>
public class RequirePermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    public RequirePermissionAttribute(string permissionCode)
    {
        Policy = permissionCode;
    }
}