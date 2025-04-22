using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using FluentValidation;
using AutoMapper;

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
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // 注册MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        // 注册FluentValidation (在最新版本中，我们需要手动注册验证器)
        // FluentValidation不再提供AddValidatorsFromAssembly扩展方法
        // 需要通过扫描程序集方式注册验证器
        var validatorType = typeof(IValidator<>);
        var validatorTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType))
            .ToList();
            
        foreach (var validator in validatorTypes)
        {
            var validatorInterface = validator.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType);
                
            services.AddScoped(validatorInterface, validator);
        }
        
        // 注册AutoMapper
        services.AddAutoMapper(assembly);
        
        return services;
    }
}