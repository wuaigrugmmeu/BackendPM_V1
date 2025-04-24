using BackendPM.Domain.Entities;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace BackendPM.Infrastructure.DataSeeding;

/// <summary>
/// 应用程序数据库初始化器，用于在系统首次启动时创建基础数据
/// </summary>
public static class AppDbInitializer
{
    /// <summary>
    /// 初始化数据库并创建种子数据
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <param name="isProduction">是否生产环境</param>
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, bool isProduction = false)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();

            // 确保数据库已创建
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("已应用数据库迁移");

            // 创建种子数据
            await SeedDataAsync(dbContext, isProduction);
            logger.LogInformation("已成功初始化数据库种子数据");
        }
        catch (DbUpdateException ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "数据库更新过程中发生错误: {Message}", ex.Message);
            throw new Exception("初始化数据库时发生数据库更新错误，请检查数据库连接和权限", ex);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "初始化数据库时发生未知错误: {Message}", ex.Message);
            throw new Exception("初始化数据库时发生错误，请查看日志获取更多信息", ex);
        }
    }

    /// <summary>
    /// 创建种子数据
    /// </summary>
    private static async Task SeedDataAsync(AppDbContext dbContext, bool isProduction = false)
    {
        // 检查是否已经有数据
        if (await dbContext.Users.AnyAsync() || await dbContext.Roles.AnyAsync() || await dbContext.Permissions.AnyAsync())
        {
            return; // 已经存在数据，不需要重新创建种子数据
        }

        // 创建基础权限
        var permissions = new List<Permission>
        {
            new("查看用户", "users.view", "用户管理", "查看用户列表和详情"),
            new("编辑角色", "roles.edit", "角色管理", "编辑角色信息"),
            new("删除角色", "roles.delete", "角色管理", "删除角色"),

            new("查看权限", "permissions.view", "权限管理", "查看权限列表和详情"),
            new("设置角色权限", "permissions.assign", "权限管理", "为角色分配权限")
        };

        // 添加权限到数据库
        await dbContext.Permissions.AddRangeAsync(permissions);
        await dbContext.SaveChangesAsync();

        // 创建角色
        var adminRole = new Role("管理员", "admin", "系统管理员，具有所有权限", !isProduction); // 在生产环境中，默认禁用敏感角色
        var userRole = new Role("普通用户", "user", "普通用户，具有基本功能权限", true);

        // 为管理员角色添加所有权限
        foreach (var permission in permissions)
        {
            adminRole.AddPermission(permission);
        }

        // 为普通用户角色添加基本权限
        var userPermissions = permissions.Where(p => p.Code is "users.view" or "roles.view" or "permissions.view").ToList();
        foreach (var permission in userPermissions)
        {
            userRole.AddPermission(permission);
        }

        // 添加角色到数据库
        await dbContext.Roles.AddRangeAsync(new List<Role> { adminRole, userRole });
        await dbContext.SaveChangesAsync();

        // 创建管理员用户
        var adminUser = new User("admin", "admin@example.com", HashPassword("Admin123!"));
        adminUser.AddRole(adminRole);

        // 创建测试用户
        var testUser = new User("user", "user@example.com", HashPassword("User123!"));
        testUser.AddRole(userRole);

        // 添加用户到数据库
        await dbContext.Users.AddRangeAsync(new List<User> { adminUser, testUser });
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 对密码进行SHA-256哈希处理
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}