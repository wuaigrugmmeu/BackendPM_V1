using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using BackendPM.Domain.Interfaces.DomainServices;
using System.Security.Claims;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 自定义权限控制属性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class PermissionAttribute : TypeFilterAttribute
{
    /// <summary>
    /// 指定所需的权限编码
    /// </summary>
    /// <param name="permissionCode">权限编码</param>
    public PermissionAttribute(string permissionCode) : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { new PermissionRequirementInfo(permissionCode) };
    }
}

/// <summary>
/// 权限需求信息
/// </summary>
public class PermissionRequirementInfo
{
    /// <summary>
    /// 权限编码
    /// </summary>
    public string PermissionCode { get; }

    public PermissionRequirementInfo(string permissionCode)
    {
        PermissionCode = permissionCode;
    }
}

/// <summary>
/// 权限验证过滤器
/// </summary>
public class PermissionFilter : IAsyncAuthorizationFilter
{
    private readonly PermissionRequirementInfo _requirement;
    private readonly IPermissionService _permissionService;

    public PermissionFilter(PermissionRequirementInfo requirement, IPermissionService permissionService)
    {
        _requirement = requirement;
        _permissionService = permissionService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 检查用户是否已认证
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 获取当前用户ID
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 检查用户是否拥有所需权限
        var hasPermission = await _permissionService.UserHasPermissionCodeAsync(userId, _requirement.PermissionCode);
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}