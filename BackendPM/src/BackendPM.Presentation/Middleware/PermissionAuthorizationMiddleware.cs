using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BackendPM.Domain.Exceptions;
using BackendPM.Domain.Interfaces.DomainServices;

namespace BackendPM.Presentation.Middleware;

/// <summary>
/// 权限验证中间件
/// </summary>
public class PermissionAuthorizationMiddleware(
    RequestDelegate next,
    ILogger<PermissionAuthorizationMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<PermissionAuthorizationMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
    {
        // 排除无需验证的路径，如登录、注册等
        if (ShouldSkipAuthorization(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // 检查用户是否已认证
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "未经授权的访问" });
            return;
        }

        // 获取当前用户ID
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            _logger.LogWarning("用户令牌中无有效的用户ID");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "无效的用户身份" });
            return;
        }

        // 获取请求路径和HTTP方法
        var path = context.Request.Path.Value;
        var method = context.Request.Method;

        try
        {
            // 检查用户是否有权限访问
            var hasPermission = await permissionService.UserHasPermissionAsync(userId, path, method);
            if (!hasPermission)
            {
                _logger.LogWarning("用户 {UserId} 尝试访问未授权资源: {Method} {Path}", userId, method, path);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "您没有权限执行此操作" });
                return;
            }

            await _next(context);
        }
        catch (AuthorizationException ex)
        {
            _logger.LogWarning(ex, "权限验证异常: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
    }

    private static bool ShouldSkipAuthorization(PathString path)
    {
        // 排除登录、注册等路径
        var excludedPaths = new[]
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/refresh-token",
            "/swagger",
            "/health"
        };

        return excludedPaths.Any(excludedPath => 
            path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase));
    }
}