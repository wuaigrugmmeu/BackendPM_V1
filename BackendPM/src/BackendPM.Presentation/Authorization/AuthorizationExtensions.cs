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
        services.AddAuthorization(options =>
        {
            // 1. 添加默认策略，要求用户通过身份验证
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                
            // 2. 添加角色策略
            options.AddPolicy("AdminRole", policy => policy.RequireRole("admin"));
            options.AddPolicy("UserRole", policy => policy.RequireRole("user"));
            
            // 3. 添加基本的权限策略
            // 用户管理权限
            options.AddPolicy("users.view", policy => 
                policy.AddRequirements(new PermissionRequirement("users.view")));
            options.AddPolicy("users.create", policy => 
                policy.AddRequirements(new PermissionRequirement("users.create")));
            options.AddPolicy("users.edit", policy => 
                policy.AddRequirements(new PermissionRequirement("users.edit")));
            options.AddPolicy("users.delete", policy => 
                policy.AddRequirements(new PermissionRequirement("users.delete")));
                
            // 角色管理权限
            options.AddPolicy("roles.view", policy => 
                policy.AddRequirements(new PermissionRequirement("roles.view")));
            options.AddPolicy("roles.create", policy => 
                policy.AddRequirements(new PermissionRequirement("roles.create")));
            options.AddPolicy("roles.edit", policy => 
                policy.AddRequirements(new PermissionRequirement("roles.edit")));
            options.AddPolicy("roles.delete", policy => 
                policy.AddRequirements(new PermissionRequirement("roles.delete")));
                
            // 权限管理权限
            options.AddPolicy("permissions.view", policy => 
                policy.AddRequirements(new PermissionRequirement("permissions.view")));
            options.AddPolicy("permissions.assign", policy => 
                policy.AddRequirements(new PermissionRequirement("permissions.assign")));
        });
        
        return services;
    }
}