using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Application.Abstractions;

/// <summary>
/// 服务自动扫描注册器
/// </summary>
public static class ServiceScanner
{
    /// <summary>
    /// 扫描并注册标记了AutoRegisterAttribute特性的服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">要扫描的程序集集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection ScanAndRegisterServices(
        this IServiceCollection services, 
        IEnumerable<Assembly> assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var typesWithAttribute = assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<AutoRegisterAttribute>() != null 
                            && !type.IsAbstract 
                            && !type.IsInterface)
                .ToList();
                
            foreach (var type in typesWithAttribute)
            {
                var attribute = type.GetCustomAttribute<AutoRegisterAttribute>();
                if (attribute == null) continue;
                
                // 获取服务类型
                Type serviceType = attribute.ServiceType ?? type;
                
                // 根据生命周期注册服务
                switch (attribute.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(serviceType, type);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(serviceType, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(serviceType, type);
                        break;
                }
            }
        }
        
        return services;
    }
    
    /// <summary>
    /// 自动注册实现了指定接口的所有服务
    /// </summary>
    /// <typeparam name="TInterface">接口类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="assemblies">要扫描的程序集集合</param>
    /// <param name="lifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection RegisterAllImplementations<TInterface>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var interfaceType = typeof(TInterface);
        
        foreach (var assembly in assemblies)
        {
            var implementations = assembly.GetTypes()
                .Where(type => interfaceType.IsAssignableFrom(type) 
                            && !type.IsAbstract 
                            && !type.IsInterface)
                .ToList();
                
            foreach (var implementation in implementations)
            {
                // 根据生命周期注册服务
                switch (lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(interfaceType, implementation);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(interfaceType, implementation);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(interfaceType, implementation);
                        break;
                }
            }
        }
        
        return services;
    }
}