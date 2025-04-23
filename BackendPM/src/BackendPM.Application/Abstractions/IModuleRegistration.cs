using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Application.Abstractions;

/// <summary>
/// 模块注册接口，各模块实现此接口以注册自己的服务
/// </summary>
public interface IModuleRegistration
{
    /// <summary>
    /// 模块优先级，数值越小优先级越高，默认为0
    /// </summary>
    int Order => 0;

    /// <summary>
    /// 注册模块中的服务
    /// </summary>
    /// <param name="services">服务集合</param>
    void RegisterModule(IServiceCollection services);
}