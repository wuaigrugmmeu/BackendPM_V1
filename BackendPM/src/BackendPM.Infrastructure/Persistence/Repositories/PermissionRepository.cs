using BackendPM.Domain.Entities;
using BackendPM.Domain.Interfaces.Repositories;
using BackendPM.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace BackendPM.Infrastructure.Persistence.Repositories;

/// <summary>
/// 权限仓储实现
/// </summary>
public class PermissionRepository : RepositoryBase<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<List<Permission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetPermissionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 获取用户所有角色的所有权限，并去重
        var permissions = await _dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .SelectMany(r => r.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async Task<Dictionary<string, List<Permission>>> GetPermissionsByGroupAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Permissions
            .OrderBy(p => p.Group)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);

        // 将权限按组分类
        return permissions
            .GroupBy(p => p.Group)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<(List<Permission> Items, int TotalCount)> GetPagedAsync(
        int pageIndex, 
        int pageSize, 
        string? searchTerm = null, 
        string? group = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<Permission> query = _dbContext.Permissions;

        // 应用分组过滤
        if (!string.IsNullOrWhiteSpace(group))
        {
            query = query.Where(p => p.Group == group);
        }

        // 应用搜索条件
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(searchTerm) || 
                p.Code.Contains(searchTerm) || 
                p.Group.Contains(searchTerm) || 
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        // 获取总记录数
        int totalCount = await query.CountAsync(cancellationToken);

        // 应用排序和分页
        var items = await query
            .OrderBy(p => p.Group)
            .ThenBy(p => p.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Permission?> FindByCodeAsync(string code)
    {
        // 调用已实现的方法，保持代码一致性
        return await GetByCodeAsync(code);
    }

    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
        // 调用已实现的方法，保持代码一致性
        return await GetPermissionsByRoleIdAsync(roleId);
    }

    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        // 调用已实现的方法，保持代码一致性
        return await GetPermissionsByUserIdAsync(userId);
    }

    public async Task<(List<Permission> Permissions, int TotalCount)> GetPagedListAsync(int pageIndex, int pageSize, string? searchTerm = null)
    {
        // 调用已实现的方法，保持代码一致性
        var result = await GetPagedAsync(pageIndex, pageSize, searchTerm);
        return (result.Items, result.TotalCount);
    }
}