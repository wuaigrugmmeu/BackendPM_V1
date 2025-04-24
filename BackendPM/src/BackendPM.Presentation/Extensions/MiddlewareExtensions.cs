using BackendPM.Presentation.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BackendPM.Presentation.Extensions;

/// <summary>
/// 中间件扩展类
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// 使用全局异常处理中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// 使用请求耗时监控中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestTimingMiddleware>();
    }

    /// <summary>
    /// 使用权限验证中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UsePermissionAuthorization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<PermissionAuthorizationMiddleware>();
    }
}