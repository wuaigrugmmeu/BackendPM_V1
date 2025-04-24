using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Presentation.Authorization;

/// <summary>
/// 授权扩展方法
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// 添加基于权限的授权
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
    {
        // 注册授权处理程序
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // 添加授权服务
        services.AddAuthorizationBuilder()
            // 添加授权服务
                             .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build())
            // 添加授权服务
                             .AddPolicy("AdminRole", policy => policy.RequireRole("admin"))
            // 添加授权服务
                             .AddPolicy("UserRole", policy => policy.RequireRole("user"))
            // 添加授权服务
                             .AddPolicy("users.view", policy =>
                policy.AddRequirements(new PermissionRequirement("users.view")))
            // 添加授权服务
                             .AddPolicy("users.create", policy =>
                policy.AddRequirements(new PermissionRequirement("users.create")))
            // 添加授权服务
                             .AddPolicy("users.edit", policy =>
                policy.AddRequirements(new PermissionRequirement("users.edit")))
            // 添加授权服务
                             .AddPolicy("users.delete", policy =>
                policy.AddRequirements(new PermissionRequirement("users.delete")))
            // 添加授权服务
                             .AddPolicy("roles.view", policy =>
                policy.AddRequirements(new PermissionRequirement("roles.view")))
            // 添加授权服务
                             .AddPolicy("roles.create", policy =>
                policy.AddRequirements(new PermissionRequirement("roles.create")))
            // 添加授权服务
                             .AddPolicy("roles.edit", policy =>
                policy.AddRequirements(new PermissionRequirement("roles.edit")))
            // 添加授权服务
                             .AddPolicy("roles.delete", policy =>
                policy.AddRequirements(new PermissionRequirement("roles.delete")))
            // 添加授权服务
                             .AddPolicy("permissions.view", policy =>
                policy.AddRequirements(new PermissionRequirement("permissions.view")))
            // 添加授权服务
                             .AddPolicy("permissions.assign", policy =>
                policy.AddRequirements(new PermissionRequirement("permissions.assign")));

        return services;
    }
}