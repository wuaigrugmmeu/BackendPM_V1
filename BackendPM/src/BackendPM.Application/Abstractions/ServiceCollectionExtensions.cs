using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using FluentValidation;
using BackendPM.Application.Behaviors.Validation;
using BackendPM.Application.Behaviors.Logging;
using BackendPM.Application.Behaviors.Transaction;
using System.Linq;
using System.Collections.Generic;

namespace BackendPM.Application.Abstractions;

/// <summary>
/// 应用层服务注册扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册应用层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="additionalAssemblies">要扫描的额外程序集</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        params Assembly[] additionalAssemblies)
    {
        // 获取当前程序集及用户提供的额外程序集
        var assemblies = GetAssemblies(additionalAssemblies);

        // 注册核心功能
        RegisterCoreServices(services, assemblies);

        // 自动扫描并注册标记了特性的服务
        services.ScanAndRegisterServices(assemblies);

        // 自动注册所有模块
        RegisterModules(services, assemblies);

        return services;
    }

    /// <summary>
    /// 获取要扫描的所有程序集
    /// </summary>
    private static List<Assembly> GetAssemblies(Assembly[] additionalAssemblies)
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };

        if (additionalAssemblies?.Length > 0)
        {
            assemblies.AddRange(additionalAssemblies);
        }

        return assemblies;
    }

    /// <summary>
    /// 注册核心服务，如MediatR、验证器等
    /// </summary>
    private static void RegisterCoreServices(IServiceCollection services, List<Assembly> assemblies)
    {
        // 注册MediatR
        services.AddMediatR(cfg =>
        {
            // 注册所有程序集中的处理器
            foreach (var assembly in assemblies)
            {
                cfg.RegisterServicesFromAssembly(assembly);
            }

            // 添加行为管道
            // 注意管道执行顺序：先验证，再记录日志，最后处理事务
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        });

        // 注册FluentValidation
        RegisterValidators(services, assemblies);

        // 注册AutoMapper (从所有程序集)
        services.AddAutoMapper(assemblies);
    }

    /// <summary>
    /// 注册验证器
    /// </summary>
    private static void RegisterValidators(IServiceCollection services, List<Assembly> assemblies)
    {
        var validatorType = typeof(IValidator<>);

        foreach (var assembly in assemblies)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType))
                .ToList();

            foreach (var validator in validatorTypes)
            {
                var validatorInterface = validator.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType);

                services.AddScoped(validatorInterface, validator);
            }
        }
    }

    /// <summary>
    /// 扫描所有程序集并注册所有模块
    /// </summary>
    private static void RegisterModules(IServiceCollection services, List<Assembly> assemblies)
    {
        // 发现所有实现了IModuleRegistration接口的类型
        var moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                typeof(IModuleRegistration).IsAssignableFrom(t))
            .ToList();

        // 按Order属性排序并实例化
        var modules = moduleTypes
            .Select(Activator.CreateInstance)
            .Cast<IModuleRegistration>()
            .OrderBy(m => m.Order)
            .ToList();

        // 注册每个模块
        foreach (var module in modules)
        {
            module.RegisterModule(services);
        }
    }
}