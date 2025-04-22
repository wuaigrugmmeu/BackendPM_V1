using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 权限授权服务扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加基于权限的授权
    /// </summary>
    public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
    {
        // 注册授权处理程序
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // 配置授权策略提供程序
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}