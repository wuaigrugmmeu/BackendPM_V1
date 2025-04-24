using Microsoft.AspNetCore.Authorization;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 基于权限的授权处理程序
/// </summary>
public class PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger = logger;

    /// <summary>
    /// 处理授权要求
    /// </summary>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 检查用户是否已认证
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            _logger.LogInformation("用户未认证，权限检查失败");
            return Task.CompletedTask;
        }

        // 从用户声明中获取权限列表
        var permissions = context.User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        // 检查用户是否拥有指定权限
        if (permissions.Contains(requirement.PermissionCode))
        {
            context.Succeed(requirement);
            _logger.LogInformation("用户拥有权限 {PermissionCode}, 授权成功", requirement.PermissionCode);
        }
        else
        {
            _logger.LogWarning(
                "用户没有所需权限 {PermissionCode}，授权失败。用户拥有的权限: {Permissions}",
                requirement.PermissionCode,
                string.Join(", ", permissions));
        }

        return Task.CompletedTask;
    }
}