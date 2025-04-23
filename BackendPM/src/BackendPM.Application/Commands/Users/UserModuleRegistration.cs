using BackendPM.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Application.Commands.Users;

/// <summary>
/// 用户模块注册类
/// </summary>
public class UserModuleRegistration : IModuleRegistration
{
    /// <summary>
    /// 用户模块优先级 - 设置为较高优先级
    /// </summary>
    public int Order => 10;

    /// <summary>
    /// 注册用户模块相关服务
    /// </summary>
    public void RegisterModule(IServiceCollection services)
    {
        // 这里可以注册特定于用户模块的服务
        // 例如: 特殊的用户服务、用户相关的仓储实现等
        
        // 示例: 如果有用户特定的服务
        // services.AddScoped<IUserSpecificService, UserSpecificService>();
        
        // 注意: 基础服务如MediatR处理器、验证器等由框架统一注册
        // 这里只需注册模块特有的服务
    }
}