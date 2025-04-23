using BackendPM.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Application.Commands.Roles;

/// <summary>
/// 角色模块注册类
/// </summary>
public class RoleModuleRegistration : IModuleRegistration
{
    /// <summary>
    /// 角色模块优先级 - 依赖于用户模块，因此优先级略低
    /// </summary>
    public int Order => 20;

    /// <summary>
    /// 注册角色模块相关服务
    /// </summary>
    public void RegisterModule(IServiceCollection services)
    {
        // 这里可以注册特定于角色模块的服务
        // 例如: 角色权限验证服务、角色管理服务等
        
        // 示例: 如果有角色特定的服务
        // services.AddScoped<IRolePermissionService, RolePermissionService>();
        
        // 注意: 基础服务如MediatR处理器、验证器等由框架统一注册
        // 这里只需注册模块特有的服务
    }
}