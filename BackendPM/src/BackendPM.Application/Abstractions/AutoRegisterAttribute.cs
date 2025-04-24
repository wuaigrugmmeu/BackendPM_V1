using System;
using Microsoft.Extensions.DependencyInjection;

namespace BackendPM.Application.Abstractions;

/// <summary>
/// 标记需要自动注册的服务
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutoRegisterAttribute : Attribute
{
    /// <summary>
    /// 服务生命周期
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// 服务接口类型，如果为null则使用实现类注册自身
    /// </summary>
    public Type? ServiceType { get; }

    /// <summary>
    /// 创建自动注册特性，使用Scoped生命周期
    /// </summary>
    public AutoRegisterAttribute() : this(ServiceLifetime.Scoped)
    {
    }

    /// <summary>
    /// 创建自动注册特性，指定生命周期
    /// </summary>
    /// <param name="lifetime">服务生命周期</param>
    public AutoRegisterAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
        ServiceType = null;
    }

    /// <summary>
    /// 创建自动注册特性，指定服务接口类型和生命周期
    /// </summary>
    /// <param name="serviceType">服务接口类型</param>
    /// <param name="lifetime">服务生命周期</param>
    public AutoRegisterAttribute(Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ServiceType = serviceType;
        Lifetime = lifetime;
    }
}