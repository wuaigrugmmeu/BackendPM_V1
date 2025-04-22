using Microsoft.AspNetCore.Authorization;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 基于权限的授权特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    public HasPermissionAttribute(string permissionCode)
        : base($"Permission{permissionCode}")
    {
        if (string.IsNullOrWhiteSpace(permissionCode))
        {
            throw new ArgumentNullException(nameof(permissionCode), "权限编码不能为空");
        }
    }
}