using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.DomainServices;
using BackendPM.Infrastructure.Persistence.DbContexts;
using System.Text.RegularExpressions;

namespace BackendPM.Infrastructure.InfrastructureServices.Permissions;

/// <summary>
/// 权限服务实现
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private const string PERMISSION_CACHE_KEY = "USER_PERMISSIONS_";
    private const string PERMISSION_CODE_CACHE_KEY = "USER_PERMISSION_CODES_";
    
    public PermissionService(AppDbContext dbContext, IMemoryCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    /// <summary>
    /// 检查用户是否有权限访问特定资源
    /// </summary>
    public async Task<bool> UserHasPermissionAsync(int userId, string? resourcePath, string? httpMethod)
    {
        // 管理员用户拥有所有权限
        if (await IsAdminUserAsync(userId))
        {
            return true;
        }
        
        // 获取用户所有权限
        var permissions = await GetUserPermissionsAsync(userId);
        
        // 如果路径为空，则无法验证
        if (string.IsNullOrEmpty(resourcePath))
        {
            return false;
        }
        
        // 首先检查精确匹配的路径权限
        var hasExactMatch = permissions.Any(p => 
            p.ResourceType == "API" && 
            p.ResourcePath == resourcePath && 
            (string.IsNullOrEmpty(p.HttpMethod) || p.HttpMethod == httpMethod)
        );
        
        if (hasExactMatch)
        {
            return true;
        }
        
        // 然后检查通配符或正则表达式匹配的路径权限
        foreach (var permission in permissions.Where(p => p.ResourceType == "API" && !string.IsNullOrEmpty(p.ResourcePath)))
        {
            // 排除非通配符/正则表达式的路径
            if (!permission.ResourcePath.Contains("*") && !permission.ResourcePath.Contains("("))
            {
                continue;
            }
            
            // 检查HTTP方法
            if (!string.IsNullOrEmpty(permission.HttpMethod) && permission.HttpMethod != httpMethod)
            {
                continue;
            }
            
            try
            {
                // 将通配符转换为正则表达式
                var pattern = "^" + Regex.Escape(permission.ResourcePath).Replace("\\*", ".*") + "$";
                if (Regex.IsMatch(resourcePath, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            catch
            {
                // 如果正则表达式解析错误，则忽略此权限
                continue;
            }
        }
        
        return false;
    }

    /// <summary>
    /// 检查用户是否拥有指定权限编码
    /// </summary>
    public async Task<bool> UserHasPermissionCodeAsync(int userId, string permissionCode)
    {
        // 管理员用户拥有所有权限
        if (await IsAdminUserAsync(userId))
        {
            return true;
        }
        
        // 获取用户所有权限编码
        var permissionCodes = await GetUserPermissionCodesAsync(userId);
        return permissionCodes.Contains(permissionCode);
    }

    /// <summary>
    /// 获取用户的所有权限编码
    /// </summary>
    public async Task<IEnumerable<string>> GetUserPermissionCodesAsync(int userId)
    {
        string cacheKey = $"{PERMISSION_CODE_CACHE_KEY}{userId}";
        
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<string> permissionCodes))
        {
            var permissions = await GetUserPermissionsAsync(userId);
            permissionCodes = permissions.Select(p => p.Code).Distinct().ToList();
            
            // 缓存30分钟
            _cache.Set(cacheKey, permissionCodes, TimeSpan.FromMinutes(30));
        }
        
        return permissionCodes;
    }

    /// <summary>
    /// 获取用户的所有权限
    /// </summary>
    public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
    {
        string cacheKey = $"{PERMISSION_CACHE_KEY}{userId}";
        
        if (!_cache.TryGetValue(cacheKey, out IEnumerable<Permission> permissions))
        {
            // 转换userId为Guid类型，因为实体使用的是Guid
            var userIdGuid = new Guid(userId.ToString("N").PadLeft(32, '0'));
            
            // 获取用户的角色关联
            var userRoles = await _dbContext.UserRoles
                .Where(ur => ur.UserId == userIdGuid)
                .ToListAsync();
            
            // 获取这些角色的所有权限
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            
            permissions = await _dbContext.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Include(rp => rp.Permission)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();
            
            // 缓存30分钟
            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(30));
        }
        
        return permissions;
    }

    /// <summary>
    /// 清除用户权限缓存
    /// </summary>
    public void ClearUserPermissionCache(int userId)
    {
        _cache.Remove($"{PERMISSION_CACHE_KEY}{userId}");
        _cache.Remove($"{PERMISSION_CODE_CACHE_KEY}{userId}");
    }
    
    /// <summary>
    /// 检查用户是否为管理员
    /// </summary>
    private async Task<bool> IsAdminUserAsync(int userId)
    {
        // 转换userId为Guid类型，因为实体使用的是Guid
        var userIdGuid = new Guid(userId.ToString("N").PadLeft(32, '0'));
        
        // 检查用户是否有管理员角色
        var isAdmin = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userIdGuid)
            .Join(_dbContext.Roles, 
                  ur => ur.RoleId, 
                  r => r.Id, 
                  (ur, r) => r.Code == "admin")
            .AnyAsync();
            
        return isAdmin;
    }
}