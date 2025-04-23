using BackendPM.Domain.Interfaces.Events;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Domain.Interfaces.DomainServices;
using BackendPM.Infrastructure.InfrastructureServices.DomainEvents;
using BackendPM.Infrastructure.InfrastructureServices.Identity;
using BackendPM.Infrastructure.InfrastructureServices.Permissions;
using BackendPM.Infrastructure.Persistence.DbContexts;
using BackendPM.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BackendPM.Infrastructure.Configuration;

/// <summary>
/// 基础设施层依赖注入扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册基础设施层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">配置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置数据库
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlite(connectionString);
        });

        // 注册领域事件分发器
        // 使用新的领域事件通知适配器，实现领域事件与中介者模式的集成
        services.AddScoped<IDomainEventDispatcher, DomainEventNotificationAdapter>();

        // 注册仓储
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // 注册权限服务
        services.AddMemoryCache(); // 确保内存缓存服务可用
        services.AddScoped<IPermissionService, PermissionService>();
        
        // 注册JWT服务
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        
        // 添加JWT认证
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"] ?? 
                        throw new InvalidOperationException("JWT Secret Key is not configured"))),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
}